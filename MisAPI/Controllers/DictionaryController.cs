﻿using Microsoft.AspNetCore.Mvc;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Controllers;

public class DictionaryController : BaseController
{
    private readonly IIcd10DictionaryService _dictionaryService;

    public DictionaryController(IIcd10DictionaryService dictionaryService)
    {
        _dictionaryService = dictionaryService;
    }

    [HttpGet("speciality")]
    public async Task<SpecialtiesPagedListModel> GetSpecialties([FromQuery] string? name, [FromQuery] int page,
        [FromQuery] int size)
    {
        var specialties = await _dictionaryService.GetSpecialtiesAsync(name, page, size);
        return specialties;
    }

    [HttpGet("icd10")]
    public async Task<Icd10SearchModel> GetIcd10Diagnoses([FromQuery] string? request, [FromQuery] int page,
        [FromQuery] int size)
    {
        var icd10Diagnoses = await _dictionaryService.GetIcd10DiagnosesAsync(request, page, size);
        return icd10Diagnoses;
    }

    [HttpGet("icd10/roots")]
    public async Task<Icd10RootsResponseModel> GetIcd10Roots()
    {
        var icd10Roots = await _dictionaryService.GetIcd10RootsAsync();
        return icd10Roots;
    }
}