﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MisAPI.Exceptions;
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
        [FromQuery] ICollection<Guid>? icdRoots = null
    )
    {
        if (start == default || end == default) throw new IncorrectDateException("Invalid date. Date cannot be empty");
        return await _reportService.GetIcdRootsReportAsync(start.ToUniversalTime(), end.ToUniversalTime(), icdRoots);
    }
}