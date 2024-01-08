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

    public ConsultationService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService)
    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
    }


    public async Task<InspectionPagedListModel> GetConsultationInspectionsAsync(IEnumerable<Guid>? icdRoots, int page,
        int size, bool grouped, Guid doctorId)
    {
        var icdRootsList = await _icd10DictionaryService.GetRootsByIcdList(icdRoots);
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
            .Include(i => i.Consultations)
            .ThenInclude(c => c.Speciality)
            .Include(i => i.Doctor)
            .Include(i => i.Patient)
            .Where(i => i.Consultations.Any(c => c.SpecialityId == specialityId || c.Speciality.Id == specialityId))
            .OrderByDescending(i => i.CreateTime)
            .AsQueryable();

        if (grouped)
        {
            inspections = inspections
                .Where(i => i.PreviousInspectionId == null || i.PreviousInspectionId == Guid.Empty);
        }

        if (icdRoots != null && icdRootsList.Any())
        {

            var icdRootsIds = icdRootsList.Select(r => r.Id);
            
            inspections = inspections
                .Where(i => i.Diagnoses != null && i.Diagnoses
                    .Where(d => d.IcdRootId != null && d.IcdRootId != Guid.Empty && d.Type == DiagnosisType.Main)
                    .Select(d => d.IcdRootId)
                    .Any(r => icdRootsIds.Contains((Guid)r!)));
        }

        var totalPages = (int)Math.Ceiling((double)await inspections.CountAsync() / size);

        if (page > totalPages)
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");

        var inspectionsList = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .AsEnumerable()
            .Select(Mapper.InspectionToInspectionPreviewModel);
        return new InspectionPagedListModel(inspectionsList, new PageInfoModel(size, totalPages, page));
    }


    public async Task<ConsultationModel> GetConsultationAsync(Guid id, Guid doctorId)
    {
        // важно что нужно получить все комментарии, пройдясь по поддереву
        var consultation = await _db.Consultations.FirstOrDefaultAsync(c => c.Id == id);
        if (consultation == null)
            throw new ConsultationNotFoundException("Consultation not found");
        var consultationSpeciality = await _db.Specialities.FirstOrDefaultAsync(s => s.Id == consultation.SpecialityId);
        if (consultationSpeciality == null)
            throw new SpecialityNotFoundException($"Speciality with id = {consultation.SpecialityId} not found");
        var comments = await GetConsultationCommentsAsync(consultation);

        return Mapper.ConsultationToConsultationModel(consultation, consultationSpeciality, comments);
    }

    private async Task<IEnumerable<Comment>> GetConsultationCommentsAsync(Consultation consultation)
    {
        var comments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.ConsultationId == consultation.Id)
            .OrderByDescending(c => c.CreateTime)
            .ToListAsync();

        await GetCommentTreeAsync(comments, consultation.RootComment);

        return comments;
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
        var doctor = await _db.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor == null) throw new DoctorNotFoundException($"Doctor with id = {doctorId} not found");
        var consultation = await _db.Consultations.FirstOrDefaultAsync(c => c.Id == consultationId);
        if (consultation == null)
            throw new ConsultationNotFoundException("Consultation not found");

        if (doctor.SpecialityId != consultation.SpecialityId)
            throw new ForbiddenLeaveCommentException(
                $"Doctor doesn't have a specialty to participate in the consultation with id = {consultationId}");
        var comment = Mapper.MapCommentCreateToComment(commentCreateModel, doctorId, consultationId);

        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();

        return new OkResult();
    }


    public async Task<IActionResult> UpdateConsultationCommentAsync(Guid commentId,
        InspectionCommentCreateModel inspectionCommentCreateModel, Guid doctorId)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null) throw new CommentNotFoundException($"Comment with id = {commentId} not found");

        if (comment.AuthorId != doctorId)
            throw new ForbiddenLeaveCommentException(
                $"Doctor with id = {doctorId} is not the author of the comment with id = {commentId}");

        comment.ModifyTime = DateTime.UtcNow;
        comment.Content = inspectionCommentCreateModel.Content;

        _db.Comments.Update(comment);
        await _db.SaveChangesAsync();

        return new OkResult();
    }
}