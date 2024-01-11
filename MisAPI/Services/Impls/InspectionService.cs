using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

public class InspectionService : IInspectionService
{
    private readonly ApplicationDbContext _db;
    private readonly IIcd10DictionaryService _icd10DictionaryService;
    private readonly ILogger<InspectionService> _logger;

    public InspectionService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService,
        ILogger<InspectionService> logger)

    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
        _logger = logger;
    }

    public async Task<InspectionModel> GetInspection(Guid id)
    {
        var inspection = await _db.Inspections
                             .Include(i => i.Patient)
                             .Include(i => i.Doctor)
                             .Include(i => i.Diagnoses)
                             .Include(i => i.Consultations)!
                             .ThenInclude(c => c.RootComment)
                             .Include(i => i.Consultations)!
                             .ThenInclude(c => c.Speciality)
                             .FirstOrDefaultAsync(i => i.Id == id)
                         ?? await _db.Inspections
                             .Include(i => i.Patient)
                             .Include(i => i.Doctor)
                             .Include(i => i.Diagnoses)
                             .Include(i => i.Consultations)
                             .FirstOrDefaultAsync(i => i.Id == id);

        if (inspection == null) throw new InspectionNotFoundException("Inspection not found.");
        
        _logger.LogInformation("Inspection with id = {id} was found", id);
        var diagnoses = inspection.Diagnoses != null
            ? inspection.Diagnoses.Select(Mapper.MapDiagnosisToDiagnosisModel)
            : new List<DiagnosisModel>();
        var consultations = inspection.Consultations != null
            ? inspection.Consultations.Select(Mapper.MapConsultationToInspectionConsultationModel)
            : new List<InspectionConsultationModel>();
        
        return Mapper.MapEntityInspectionToInspectionModel(inspection, diagnoses, consultations);
    }


    public async Task<IActionResult> EditInspection(Guid id, InspectionEditModel inspectionEditModel, Guid doctorId)
    {
        var inspectionEntity = await _db.Inspections
            .Include(i => i.Diagnoses)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (inspectionEntity == null) throw new InspectionNotFoundException($"Inspection with id = {id} not found");
        _logger.LogInformation("Inspection with id = {id} was found", id);
        await ValidateInspectionEntity(inspectionEntity, doctorId);
        await ValidateConclusionAndDates(
            Mapper.MapInspectionEditModelToConclusionAndDateValidationModel(inspectionEditModel));

        await ValidateDiagnoses(inspectionEditModel.Diagnoses);
        if (inspectionEntity.Diagnoses != null)
        {
            _db.Diagnoses.RemoveRange(inspectionEntity.Diagnoses);
        }
        
        var diagnoses = await MapDiagnosesAsync(id, inspectionEditModel.Diagnoses.ToList());
        inspectionEntity.Diagnoses = diagnoses;
        var updatedInspectionEntity = Mapper.GetUpdatedInspectionEntity(inspectionEntity, inspectionEditModel);
        _db.Inspections.Update(updatedInspectionEntity);
        await _db.Diagnoses.AddRangeAsync(diagnoses);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Edit model is correct. So inspection was updated successfully");

        return new OkResult();
    }

    public async Task<ICollection<Diagnosis>> MapDiagnosesAsync(Guid inspectionId,
        ICollection<DiagnosisCreateModel> diagnosisCreateModels)
    {
        var diagnoses = new List<Diagnosis>();

        foreach (var diagnosisCreateModel in diagnosisCreateModels)
        {
            
            var diagnosisFromDb =
                await _icd10DictionaryService.GetIcd10DiagnosisAsync(diagnosisCreateModel.IcdDiagnosisId);
            var diagnosisEntity = Mapper.MapDiagnosisCreateModelToDiagnosis(diagnosisCreateModel);
            diagnosisEntity.Code = diagnosisFromDb.Code;
            diagnosisEntity.Name = diagnosisFromDb.Name;
            diagnosisEntity.InspectionId = inspectionId;
            diagnosisEntity.IcdRootId = diagnosisFromDb.IcdRootId;

            diagnoses.Add(diagnosisEntity);
        }

        return diagnoses;
    }

    private static Task ValidateInspectionEntity(Inspection inspectionEntity, Guid doctorId)
    {
        if (inspectionEntity == null) throw new InspectionNotFoundException("Inspection not found.");
        if (inspectionEntity.DoctorId != doctorId)
            throw new NotHavePermissionException(
                $"Doctor with id {doctorId} not have permission to edit inspection with id {inspectionEntity.Id}");
        return Task.CompletedTask;
    }

    public Task ValidateConclusionAndDates(ConclusionAndDateValidationModel validationModel)
    {
        return validationModel.Conclusion switch
        {
            Conclusion.Disease when validationModel.NextVisitDate == null => throw
                new InvalidValueForAttributeDateException("Next visit date cannot be null if conclusion is disease"),
            Conclusion.Death when validationModel.DeathDate == null => throw
                new InvalidValueForAttributeDateException("Death date cannot be null if conclusion is death"),
            Conclusion.Recovery when validationModel.DeathDate != null => throw
                new InvalidValueForAttributeDateException("Death date must be null if conclusion is recovery"),
            Conclusion.Recovery when validationModel.NextVisitDate != null => throw
                new InvalidValueForAttributeDateException("Next visit date must be null if conclusion is recovery"),
            Conclusion.Disease when validationModel.DeathDate != null => throw
                new InvalidValueForAttributeDateException("Death date must be null if conclusion is disease"),
            Conclusion.Recovery when validationModel.DeathDate != null => throw
                new InvalidValueForAttributeDateException("Death date must be null if conclusion is recovery"),
            _ => Task.CompletedTask
        };
    }

    public Task ValidateDiagnoses(ICollection<DiagnosisCreateModel> diagnoses)
    {
        var diagnosesList = diagnoses.ToList();
        if (diagnoses == null || !diagnosesList.Any())
            throw new InvalidValueForAttributeDiagnosesException("Diagnoses cannot be null or empty");
        if (diagnosesList.Count(d => d.Type == DiagnosisType.Main) != 1)
            throw new InvalidValueForAttributeDiagnosesException("Inspection must have one main diagnosis");
        return Task.CompletedTask;
    }


    public async Task<ICollection<InspectionPreviewModel>> GetInspectionChain(Guid id)
    {
        var rootInspection = await _db.Inspections
            .Include(inspection => inspection.Diagnoses)
            .Include(inspection => inspection.Doctor)
            .Include(inspection => inspection.Patient)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (rootInspection == null) throw new InspectionNotFoundException("Inspection not found.");
        if (rootInspection.PreviousInspectionId != null || rootInspection.BaseInspectionId != null)
            throw new InspectionIsNotRootException("Inspection is not root.");
        _logger.LogInformation("Root inspection successfully found");
        var inspections = new List<InspectionPreviewModel>();
        var currentInspection = await _db.Inspections
            .Include(inspection => inspection.Diagnoses)
            .Include(inspection => inspection.Doctor)
            .Include(inspection => inspection.Patient)
            .FirstOrDefaultAsync(i => i.PreviousInspectionId == rootInspection.Id);

        while (currentInspection != null)
        {
            var diagnosis = currentInspection.Diagnoses?
                .FirstOrDefault(d => d.Type == DiagnosisType.Main);
            var diagnosisModel = diagnosis != null ? Mapper.MapDiagnosisToDiagnosisModel(diagnosis) : null;

            var hasChain = currentInspection.BaseInspectionId == null;
            var hasNested = await _db.Inspections.AnyAsync(i => i.PreviousInspectionId == currentInspection.Id);

            inspections.Add(Mapper.MapEntityInspectionToInspectionPreviewModel(currentInspection, diagnosisModel,
                hasChain, hasNested));
            currentInspection = await _db.Inspections
                .Include(inspection => inspection.Diagnoses)
                .Include(inspection => inspection.Doctor)
                .Include(inspection => inspection.Patient)
                .FirstOrDefaultAsync(i => i.PreviousInspectionId == currentInspection.Id);
        }
        _logger.LogInformation("All inspections from chain were collected");
        return inspections;
    }
}