using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Enums;
using MisAPI.Exceptions;
using MisAPI.Mappers;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class ConsultationService : IConsultationService
{
    private readonly ApplicationDbContext _db;
    private readonly IIcd10DictionaryService _icd10DictionaryService;
    private readonly ILogger<ConsultationService> _logger;

    public ConsultationService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService,
        ILogger<ConsultationService> logger)

    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
        _logger = logger;
    }


    public async Task<InspectionPagedListModel> GetConsultationInspectionsAsync(ICollection<Guid>? icdRoots, int page,
        int size, bool grouped, Guid doctorId)
    {
        var icdRootsList = icdRoots != null
            ? await _icd10DictionaryService.GetRootsByIcdList(icdRoots.ToList())
            : new List<Icd10RootModel>();
        var specialityId = await _db.Doctors
            .Where(d => d.Id == doctorId)
            .Select(d => d.SpecialityId)
            .FirstOrDefaultAsync();
        if (specialityId == Guid.Empty)
            throw new IncorrectSpecialityException("Doctor has no speciality");

        if (!_db.Specialities.Any(s => s.Id == specialityId))
            throw new IncorrectSpecialityException("Doctor has no speciality");


        var inspections = _db.Inspections
            .Include(i => i.Diagnoses)
            .Include(i => i.Consultations)!
            .ThenInclude(c => c.Speciality)
            .Include(i => i.Doctor)
            .Include(i => i.Patient)
            .Where(i => i.Consultations != null && i.Consultations.Any(c =>
                c.SpecialityId == specialityId || c.Speciality.Id == specialityId))
            .OrderByDescending(i => i.CreateTime)
            .AsQueryable();

        if (grouped)
        {
            inspections = inspections
                .Where(i => i.PreviousInspectionId == null || i.PreviousInspectionId == Guid.Empty);
        }

        if (icdRootsList.Any())
        {
            var icdRootsIds = icdRootsList.Select(r => r.Id);

            inspections = inspections
                .Where(i => i.Diagnoses != null && i.Diagnoses
                    .Where(d => d.IcdRootId != null && d.IcdRootId != Guid.Empty && d.Type == DiagnosisType.Main)
                    .Select(d => d.IcdRootId)
                    .Any(r => icdRootsIds.Contains((Guid)r!)));
        }

        var totalPages = (int)Math.Ceiling((double)await inspections.CountAsync() / size);

        if (page != 1 && page > totalPages)
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");
        _logger.LogInformation("Inspections for consultations with specialityId = {specialityId} were collected.",
            specialityId);
        var inspectionsList = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .AsEnumerable()
            .Select(Mapper.InspectionToInspectionPreviewModel)
            .ToList();
        return new InspectionPagedListModel(inspectionsList, new PageInfoModel(size, totalPages, page));
    }


    public async Task<ConsultationModel> GetConsultationAsync(Guid id)
    {
        var consultation = await _db.Consultations
                               .Include(c => c.RootComment)
                               .FirstOrDefaultAsync(c => c.Id == id)
                           ?? throw new ConsultationNotFoundException("Consultation not found");

        var consultationSpeciality = await _db.Specialities
                                         .FirstOrDefaultAsync(s => s.Id == consultation.SpecialityId)
                                     ?? throw new SpecialityNotFoundException(
                                         $"Speciality with id = {consultation.SpecialityId} not found");
        _logger.LogInformation("Consultation with id = {id} successfully found", id);
        var comments = await GetConsultationCommentsAsync(consultation);

        return Mapper.ConsultationToConsultationModel(consultation, consultationSpeciality, comments);
    }

    private async Task<ICollection<Comment>> GetConsultationCommentsAsync(Consultation consultation)
    {
        var comments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.ConsultationId == consultation.Id)
            .OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        await GetCommentTreeAsync(comments, consultation.RootComment);

        return comments.OrderBy(c => c.CreateTime).ToList();
    }

    private async Task GetCommentTreeAsync(ICollection<Comment> comments, Comment rootComment)
    {
        var stack = new Stack<Comment>();
        await GetSubCommentsAsync(stack, rootComment);
        while (stack.Count > 0)
        {
            var currentComment = stack.Pop();
            comments.Add(currentComment);
            await GetSubCommentsAsync(stack, currentComment);
        }
    }

    private async Task GetSubCommentsAsync(Stack<Comment> stack, Comment comment)
    {
        var subComments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.ParentId == comment.Id)
            .OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        foreach (var subComment in subComments)
        {
            stack.Push(subComment);
        }
    }

    public async Task<IActionResult> AddCommentToConsultationAsync(Guid consultationId,
        CommentCreateModel commentCreateModel, Guid doctorId)
    {
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId)
                     ?? throw new DoctorNotFoundException($"Doctor with id = {doctorId} not found");

        var consultation = await _db.Consultations.FirstOrDefaultAsync(c => c.Id == consultationId)
                           ?? throw new ConsultationNotFoundException("Consultation not found");

        if (doctor.SpecialityId != consultation.SpecialityId)
            throw new ForbiddenLeaveCommentException(
                $"Doctor doesn't have a specialty to participate in the consultation with id = {consultationId}");

        var parentComment = await _db.Comments
                                .Include(c => c.Children)
                                .FirstOrDefaultAsync(c => c.Id == commentCreateModel.ParentId)
                            ?? throw new CommentNotFoundException(
                                $"Comment with id = {commentCreateModel.ParentId} not found");

        if (parentComment.ConsultationId != consultationId)
            throw new ForbiddenLeaveCommentException(
                $"Comment with id = {commentCreateModel.ParentId} is not a comment of the consultation with id = {consultationId}");


        var comment = Mapper.MapCommentCreateToComment(commentCreateModel, doctorId, consultationId);
        consultation.CommentsNumber++;

        parentComment.Children?.Add(comment);
        await _db.Comments.AddAsync(comment);
        _db.Comments.Update(parentComment);
        _db.Consultations.Update(consultation);

        await _db.SaveChangesAsync();
        _logger.LogInformation("Comment to consultation with id = {id} and comment parentId = {parentId} was added",
            consultationId, parentComment.Id);
        return new OkResult();
    }


    public async Task<IActionResult> UpdateConsultationCommentAsync(Guid commentId,
        InspectionCommentCreateModel inspectionCommentCreateModel, Guid doctorId)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId)
                      ?? throw new CommentNotFoundException($"Comment with id = {commentId} not found");

        if (comment.AuthorId != doctorId)
            throw new ForbiddenLeaveCommentException(
                $"Doctor with id = {doctorId} is not the author of the comment with id = {commentId}");

        comment.ModifyTime = DateTime.UtcNow;
        comment.Content = inspectionCommentCreateModel.Content;

        _db.Comments.Update(comment);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Comment with id = {id} was updated", commentId);
        return new OkResult();
    }
}