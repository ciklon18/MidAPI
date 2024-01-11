using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using MisAPI.Exceptions;
using MisAPI.Models.Api;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class Icd10DictionaryService : IIcd10DictionaryService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<Icd10DictionaryService> _logger;

    public Icd10DictionaryService(ApplicationDbContext db, ILogger<Icd10DictionaryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<SpecialtiesPagedListModel> GetSpecialtiesAsync(string? name, int page, int size)
    {
        var lowerName = name?.ToLower();
        var specialties = _db.Specialities
            .Where(s => lowerName == null || s.Name.ToLower().Contains(lowerName));
        
        var totalPages = (int)Math.Ceiling((double)await specialties.CountAsync() / size);
        if (page != 1 && page > totalPages)
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");
        
        _logger.LogInformation("Required specialties was found");
        var selectedModelSpecialties = await specialties
            .Skip((page - 1) * size)
            .Take(size)
            .Select(s => new SpecialityModel(s.Id, s.Name, s.CreateTime))
            .ToListAsync();
        
        var specialtiesPagedListModel = new SpecialtiesPagedListModel
        {
            Specialities = selectedModelSpecialties,
            Pagination = new PageInfoModel(size, totalPages, page)
        };
        return specialtiesPagedListModel;
    }

    public async Task<Icd10SearchModel> GetIcd10DiagnosesAsync(string? request, int page, int size)
    {
        var lowerRequest = request != null ? request.ToLower() : string.Empty;
        var diagnoses = _db.Icd10
            .Where(d => d.IcdCode.ToLower().Contains(lowerRequest) || d.IcdName.ToLower().Contains(lowerRequest));

        var totalPages = (int)Math.Ceiling((double)await diagnoses.CountAsync() / size);
        if (page != 1 && page > totalPages)
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");
        _logger.LogInformation("Required diagnoses was found");

        var recordDiagnoses = await diagnoses
            .Skip((page - 1) * size)
            .Take(size)
            .Select(d => new Icd10RecordModel(d.IdGuid ?? new Guid(), d.CreateTime, d.IcdCode, d.IcdName))
            .ToListAsync();

        var pagination = new PageInfoModel(size, totalPages, page);
        return new Icd10SearchModel( recordDiagnoses, pagination);
    }

    public async Task<Icd10RootsResponseModel> GetIcd10RootsAsync()
    {
        var roots = await _db.Icd10Roots
            .Select(r => new Icd10RecordModel(r.Id, r.CreateTime, r.Code, r.Name))
            .ToListAsync();
        _logger.LogInformation("Roots successfully loaded");
        return new Icd10RootsResponseModel(roots);
    }

    public async Task<ExtendedDiagnosisModel> GetIcd10DiagnosisAsync(Guid icdDiagnosisId)
    {
        var icd10 = await _db.Icd10.FirstOrDefaultAsync(d => d.IdGuid == icdDiagnosisId);
        if (icd10 == null) throw new DiagnosisNotFoundException($"Diagnosis with id = {icdDiagnosisId} not found");
        _logger.LogInformation("Diagnosis with id = {id} was found", icdDiagnosisId);
        return new ExtendedDiagnosisModel
        {
            Id = icd10.IdGuid ?? new Guid(),
            CreateTime = icd10.CreateTime,
            Code = icd10.IcdCode,
            Name = icd10.IcdName,
            IcdRootId = icd10.RootIdGuid
        };
    }
    


    public async Task CheckAreIcdRootsExist(ICollection<Guid>? icdRoots)
    {
        if (icdRoots == null) return;
        var icdRootsList = _db.Icd10Roots.AsQueryable();
        foreach (var icdRoot in icdRoots)
        {
            if (!await icdRootsList.AnyAsync(i => i.Id == icdRoot))
                throw new IcdRootNotFoundException($"Icd root with id = {icdRoot} not found");
        }
    }

    public async Task<ICollection<Icd10RootModel>> GetRootsByIcdList(ICollection<Guid>? icdRoots)
    {
        if (icdRoots == null) return new List<Icd10RootModel>();

        var existingRoots = await _db.Icd10Roots
            .Where(i => icdRoots.Contains(i.Id))
            .ToListAsync();

        var nonExistingRoots = icdRoots
            .Except(existingRoots.Select(r => r.Id))
            .ToList();
        if (nonExistingRoots.Any())
        {
            throw new IcdRootNotFoundException($"Icd roots with ids {string.Join(", ", nonExistingRoots)} not found");
        }

        return existingRoots
            .Select(r => new Icd10RootModel(r.Id, r.CreateTime, r.Code ?? string.Empty, r.Name))
            .ToList();
    }

    public async Task<ICollection<Icd10RootModel>> GetRootsWithoutIcdList()
    {
        return await _db.Icd10Roots
            .Select(r => new Icd10RootModel(r.Id, r.CreateTime, r.Code ?? "", r.Name))
            .ToListAsync();
    }
}