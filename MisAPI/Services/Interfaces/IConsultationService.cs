using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IConsultationService
{
    Task<InspectionPagedListModel> GetConsultationInspectionsAsync(ICollection<Guid>? icdRoots, int page, int size, bool grouped, Guid doctorId);
    Task<ConsultationModel> GetConsultationAsync(Guid id);
    Task<IActionResult> AddCommentToConsultationAsync(Guid consultationId, CommentCreateModel commentCreateModel, Guid doctorId);
    Task<IActionResult> UpdateConsultationCommentAsync(Guid consultationId, InspectionCommentCreateModel inspectionCommentCreateModel, Guid doctorId);
    
}