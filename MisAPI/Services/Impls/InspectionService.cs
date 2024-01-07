using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using MisAPI.Enums;
using MisAPI.Exceptions;
using MisAPI.Mappers;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class InspectionService : IInspectionService
{
    private readonly ApplicationDbContext _db;

    public InspectionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<InspectionModel> GetInspection(Guid id, Guid doctorId)
    {
        var inspection = await _db.Inspections
            .Include(i => i.Patient)
            .Include(i => i.Doctor)
            .Include(i => i.Diagnoses)
            .Include(i => i.Consultations)
            .FirstOrDefaultAsync(i => i.Id == id);
        if (inspection == null) throw new InspectionNotFoundException("Inspection not found.");

        return Mapper.MapEntityInspectionToInspectionModel(inspection);
    }

    public async Task<IActionResult> EditInspection(Guid id, InspectionEditModel inspection, Guid doctorId)
    {
        var inspectionEntity = await _db.Inspections.FirstOrDefaultAsync(i => i.Id == id);
        
        if (inspectionEntity == null) throw new InspectionNotFoundException("Inspection not found.");
        if (inspectionEntity.DoctorId != doctorId)
            throw new NotHavePermissionException($"Doctor with id {doctorId} not have permission to edit inspection with id {id}.");
        await CheckAreDiagnosesExist(inspection.Diagnoses);
        
        var updatedInspectionEntity = Mapper.GetUpdatedInspectionEntity(inspectionEntity, inspection);
        
        _db.Update(updatedInspectionEntity);
        await _db.SaveChangesAsync();
        
        return new OkResult();
    }

    private async Task CheckAreDiagnosesExist(IEnumerable<DiagnosisCreateModel> inspectionDiagnoses)
    {
        foreach (var diagnosis in inspectionDiagnoses)
        {
            var diagnosisEntity = await _db.Diagnoses.FirstOrDefaultAsync(d => d.Id == diagnosis.IcdDiagnosisId);
            if (diagnosisEntity == null) throw new DiagnosisNotFoundException($"Diagnosis with id {diagnosis.IcdDiagnosisId} not found.");
        }
    }

    public async Task<IEnumerable<InspectionPreviewModel>> GetInspectionChain(Guid id, Guid doctorId)
    {
        var rootInspection = await _db.Inspections
            .Include(inspection => inspection.Diagnoses)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (rootInspection == null) throw new InspectionNotFoundException("Inspection not found.");
        if (rootInspection.PreviousInspectionId != null || rootInspection.BaseInspectionId != null)
            throw new InspectionIsNotRootException("Inspection is not root.");

        var inspections = new List<InspectionPreviewModel>();
        var currentInspection = rootInspection;
        while (currentInspection != null)
        {
            var diagnosis = currentInspection.Diagnoses?
                .FirstOrDefault(d => d.Type == DiagnosisType.Main);
            var diagnosisModel = diagnosis != null ? Mapper.MapDiagnosisToDiagnosisModel(diagnosis) : null;

            var hasChain = currentInspection.BaseInspectionId == null;
            var hasNested = await _db.Inspections.AnyAsync(i => i.PreviousInspectionId == currentInspection.Id);
            
            inspections.Add(Mapper.MapEntityInspectionToInspectionPreviewModel(currentInspection, diagnosisModel, hasChain, hasNested));
            currentInspection = await _db.Inspections
                .Include(inspection => inspection.Diagnoses)
                .FirstOrDefaultAsync(i => i.PreviousInspectionId == currentInspection.Id);
        }

        return inspections;
    }
}