using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Services.Interfaces;

namespace MisAPI.Controllers;

public class InspectionController : AuthorizeController
{
    private readonly IInspectionService _inspectionService;
    
    public InspectionController(IInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    
    
    [HttpGet("{id:guid}")]
    public async Task<InspectionModel> GetInspection([FromRoute] Guid id)
    {
        return await _inspectionService.GetInspection(id, DoctorId);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutInspection([FromRoute] Guid id, [FromBody] InspectionEditModel inspection)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return await _inspectionService.EditInspection(id, inspection, DoctorId);
    }
    
    [HttpGet("{id:guid}/chain")]
    public async Task<IEnumerable<InspectionPreviewModel>> GetInspectionChain([FromRoute] Guid id)
    {
        return await _inspectionService.GetInspectionChain(id, DoctorId);
    }

}