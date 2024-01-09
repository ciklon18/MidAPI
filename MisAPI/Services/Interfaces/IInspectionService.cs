using Microsoft.AspNetCore.Mvc;
using MisAPI.Entities;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IInspectionService
{
    Task<InspectionModel> GetInspection(Guid id);
    Task<IActionResult> EditInspection(Guid id, InspectionEditModel inspectionEditModel, Guid doctorId);
    Task<ICollection<InspectionPreviewModel>> GetInspectionChain(Guid id);

    Task<ICollection<Diagnosis>> MapDiagnosesAsync(Guid inspectionId,
        ICollection<DiagnosisCreateModel> diagnosisCreateModels);

    Task ValidateConclusionAndDates(ConclusionAndDateValidationModel validationModel);
    Task ValidateDiagnoses(ICollection<DiagnosisCreateModel> diagnoses);

}