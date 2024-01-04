﻿using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using MisAPI.Exceptions;
using MisAPI.Models.Api;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class Icd10DictionaryService : IIcd10DictionaryService
{
    private readonly ApplicationDbContext _db;

    public Icd10DictionaryService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<SpecialtiesPagedListModel> GetSpecialtiesAsync(string? name, int page, int size)
    {
        var lowerName = name?.ToLower();
        var specialties = _db.Specialties
            .Where(s => lowerName == null || s.Name.ToLower().Contains(lowerName));
        var specialtiesCount = await specialties.CountAsync();
        var selectedSpecialties = specialties
            .Skip((page - 1) * size)
            .Take(size);
        if (page > specialtiesCount / size && !await selectedSpecialties.AnyAsync())
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");
        var selectedModelSpecialties = await selectedSpecialties
            .Select(s => new SpecialityModel(s.Id, s.Name, s.CreateTime))
            .ToListAsync();

        var count = specialtiesCount / size;
        if (specialtiesCount % size != 0) count++;
        var specialtiesPagedListModel = new SpecialtiesPagedListModel
        {
            Specialities = selectedModelSpecialties,
            Pagination = new PageInfoModel(size, count, page)
        };
        return specialtiesPagedListModel;
    }

    public async Task<Icd10SearchModel> GetIcd10DiagnosesAsync(string? request, int page, int size)
    {
        var lowerRequest = request != null ? request.ToLower() : "";
        var diagnoses = _db.Mkb10
            .Where(d => d.MkbCode.ToLower().Contains(lowerRequest) || d.MkbName.ToLower().Contains(lowerRequest));
        var diagnosesCount = await diagnoses.CountAsync();
        var selectedDiagnoses = diagnoses
            .Skip((page - 1) * size)
            .Take(size);
        if (page > diagnosesCount / size && !await selectedDiagnoses.AnyAsync())
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");

        var recordDiagnoses = selectedDiagnoses.Select(d =>
            new Icd10RecordModel(d.IdGuid ?? new Guid(), d.CreateTime, d.MkbCode, d.MkbName));

        
        var count = diagnosesCount / size;
        if (diagnosesCount % size != 0) count++;

        var pagination = new PageInfoModel(size, count, page);
        return new Icd10SearchModel(await recordDiagnoses.ToListAsync(), pagination);
    }

    public async Task<Icd10RootsResponseModel> GetIcd10RootsAsync()
    {
        var roots = await _db.Mkb10Roots
            .Select(r => new Icd10RecordModel(r.Id, r.CreateTime, r.Code, r.Name))
            .ToListAsync();
        return new Icd10RootsResponseModel(roots);
    }

    public async Task<DiagnosisModel> GetIcd10DiagnosisAsync(Guid icdDiagnosisId)
    {
        var diagnosis = await _db.Mkb10.FirstOrDefaultAsync(d => d.IdGuid == icdDiagnosisId);
        if (diagnosis == null) throw new DiagnosisNotFoundException($"Diagnosis with id = {icdDiagnosisId} not found");
        return new DiagnosisModel
        {
            Id = diagnosis.IdGuid ?? new Guid(),
            CreateTime = diagnosis.CreateTime,
            Code = diagnosis.MkbCode,
            Name = diagnosis.MkbName
        };
    }
}