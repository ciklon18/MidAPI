using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;
using MisAPI.Validator;

namespace MisAPI.Controllers;

public class ReportController : AuthorizeController
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("icdrootsreport")]
    public async Task<IcdRootsReportModel> GetIcdRootsReport(
        [Required] [FromQuery] [DateValidator] DateTime start,
        [Required] [FromQuery] [DateValidator] DateTime end,
        [FromQuery] IEnumerable<Guid>? icdRoots = null
    )
    {
        var report = await _reportService.GetIcdRootsReportAsync(start, end, icdRoots, DoctorId);
        return report;
    }
}