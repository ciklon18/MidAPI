using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IInspectionService
{
    Task<InspectionModel> GetInspection(Guid id, Guid doctorId);
    Task<IActionResult> PutInspection(Guid id, InspectionEditModel inspection, Guid doctorId);
    Task<IEnumerable<InspectionPreviewModel>> GetInspectionChain(Guid id, Guid doctorId);
}