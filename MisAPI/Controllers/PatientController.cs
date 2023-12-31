using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MisAPI.Enums;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Controllers;

public class PatientController : AuthorizeController
{
    private readonly IPatientService _patientService;

    public PatientController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] PatientCreateModel patientCreateModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await _patientService.CreatePatient(patientCreateModel, DoctorId));
    }

    [HttpGet]
    public Task<PatientPagedListModel> GetPatients(
        [FromQuery] string? name = null,
        [FromQuery] [EnumDataType(typeof(Conclusion), ErrorMessage = "Invalid conclusion value.")]
        Conclusion[]? conclusions = null,
        [FromQuery] PatientSorting sorting = PatientSorting.NameAsc,
        [FromQuery] bool scheduledVisits = false,
        [FromQuery] bool onlyMine = false,
        [FromQuery] [Range(1, int.MaxValue)] int page = 1,
        [FromQuery] [Range(1, int.MaxValue)] int size = 5
    )
    {
        return _patientService.GetPatients(name, conclusions, sorting, scheduledVisits, onlyMine, page, size, DoctorId);
    }


    [HttpPost("{id:guid}/inspections")]
    public async Task<IActionResult> CreateInspection([FromRoute] Guid id,
        [FromBody] InspectionCreateModel inspectionCreateModel)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        return Ok(await _patientService.CreateInspection(id, inspectionCreateModel, DoctorId));
    }

    [HttpGet("{id:guid}/inspections")]
    public Task<InspectionPagedListModel> GetPatientInspections(
        [FromRoute] Guid id,
        [FromQuery] bool grouped = false,
        [FromQuery] IEnumerable<Guid>? icdRoots = null,
        [FromQuery] [Range(1, int.MaxValue)] int page = 1,
        [FromQuery] [Range(1, int.MaxValue)] int size = 5
    )
    {
        return _patientService.GetInspections(id, grouped, icdRoots, page, size, DoctorId);
    }
    
    [HttpGet("{id:guid}")]
    public Task<PatientModel> GetPatientCard([FromRoute] Guid id)
    {
        return _patientService.GetPatientCard(id, DoctorId);
    }
    
    [HttpGet("{id:guid}/inspections/search")]
    public Task<InspectionShortModel> SearchInspections([Required][FromRoute] Guid id, [FromQuery] string? request)
    {
        return _patientService.SearchInspections(id, request, DoctorId);
    }
}