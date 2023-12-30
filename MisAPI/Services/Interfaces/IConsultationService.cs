using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IConsultationService
{
    Task<InspectionPagedListModel> GetConsultationsAsync(IEnumerable<Guid>? icdRoots, int page, int size, bool grouped);
    Task<ConsultationModel> GetConsultationAsync(Guid id);
    Task<IActionResult> AddCommentToConsultationAsync(Guid consultationId, CommentCreateModel commentCreateModel);
    Task<IActionResult> UpdateConsultationAsync(Guid consultationId, InspectionCommentCreateModel inspectionCommentCreateModel);
    
}