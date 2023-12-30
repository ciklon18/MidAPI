using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Controllers;

public class ConsultationController : AuthorizeController
{
    private readonly IConsultationService _consultationService;

    public ConsultationController(IConsultationService consultationService)
    {
        _consultationService = consultationService;
    }

    [HttpGet]
    public async Task<InspectionPagedListModel> GetConsultations(
        [FromQuery] bool grouped = false,
        [FromQuery] IEnumerable<Guid>? icdRoots = null,
        [FromQuery] [Range(1, int.MaxValue)] int page = 1,
        [FromQuery] [Range(1, int.MaxValue)] int size = 5
    )
    {
        return await _consultationService.GetConsultationsAsync(icdRoots, page, size, grouped);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ConsultationModel> GetConsultation([FromRoute] Guid id)
    {
        var consultation = await _consultationService.GetConsultationAsync(id);
        return consultation;
    }

    [HttpPost("{id:Guid}/comment")]
    public async Task<IActionResult> AddCommentToConsultation([FromRoute] Guid id, [FromBody] CommentCreateModel commentCreateModel)
    {
        return await _consultationService.AddCommentToConsultationAsync(id, commentCreateModel);
    }

    [HttpPut("comment/{id:Guid}")]
    public async Task<IActionResult> UpdateConsultation([FromRoute] Guid id, [FromBody] InspectionCommentCreateModel inspectionCommentCreateModel)
    {
        return await _consultationService.UpdateConsultationAsync(id, inspectionCommentCreateModel);
    }
}