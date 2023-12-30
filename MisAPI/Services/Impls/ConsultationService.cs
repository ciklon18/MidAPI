using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class ConsultationService : IConsultationService
{
    public Task<InspectionPagedListModel> GetConsultationsAsync(IEnumerable<Guid>? icdRoots, int page, int size, bool grouped)
    {
        throw new NotImplementedException();
    }

    public Task<ConsultationModel> GetConsultationAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> AddCommentToConsultationAsync(Guid consultationId, CommentCreateModel commentCreateModel)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> UpdateConsultationAsync(Guid consultationId, InspectionCommentCreateModel inspectionCommentCreateModel)
    {
        throw new NotImplementedException();
    }
}