using Microsoft.AspNetCore.Mvc;
using MisAPI.Entities;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IInspectionService
{
    Task<InspectionModel> GetInspection(Guid id, Guid doctorId);
    Task<IActionResult> EditInspection(Guid id, InspectionEditModel inspectionEditModel, Guid doctorId);
    Task<IEnumerable<InspectionPreviewModel>> GetInspectionChain(Guid id, Guid doctorId);

    Task<List<Diagnosis>> MapDiagnosesAsync(Guid inspectionId,
        IEnumerable<DiagnosisCreateModel> diagnosisCreateModels);

    Task ValidateConclusionAndDates(ConclusionAndDateValidationModel validationModel);
    Task ValidateDiagnoses(IEnumerable<DiagnosisCreateModel> diagnoses);

}