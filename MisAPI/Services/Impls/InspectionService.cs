using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class InspectionService : IInspectionService
{
    public Task<InspectionModel> GetInspection(Guid id, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> PutInspection(Guid id, InspectionEditModel inspection, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionPreviewModel> GetInspectionChain(Guid id, Guid doctorId)
    {
        throw new NotImplementedException();
    }
}