using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
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
        [FromQuery] ICollection<Guid>? icdRoots = null,
        [FromQuery] [Range(1, int.MaxValue)] int page = 1,
        [FromQuery] [Range(1, int.MaxValue)] int size = 5
    )
    {
        return await _consultationService.GetConsultationInspectionsAsync(icdRoots, page, size, grouped, DoctorId);
    }

    [HttpGet("{id:Guid}")]
    public async Task<ConsultationModel> GetConsultation([FromRoute] Guid id)
    {
        var consultation = await _consultationService.GetConsultationAsync(id);
        return consultation;
    }

    [HttpPost("{id:Guid}/comment")]
    public async Task<IActionResult> AddCommentToConsultation([FromRoute] Guid id,
        [FromBody] CommentCreateModel commentCreateModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _consultationService.AddCommentToConsultationAsync(id, commentCreateModel, DoctorId);
    }

    [HttpPut("comment/{id:Guid}")]
    public async Task<IActionResult> UpdateConsultationComment(
        [FromRoute] Guid id,
        [FromBody] InspectionCommentCreateModel inspectionCommentCreateModel
    )
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _consultationService.UpdateConsultationCommentAsync(id, inspectionCommentCreateModel, DoctorId);
    }
}