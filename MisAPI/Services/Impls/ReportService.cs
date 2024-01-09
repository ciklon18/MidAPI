using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Models.Api;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _db;
    private readonly IIcd10DictionaryService _icd10DictionaryService;

    public ReportService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService)
    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
    }

    public async Task<IcdRootsReportModel> GetIcdRootsReportAsync(DateTime start, DateTime end,
        ICollection<Guid>? icdRoots)
    {
        if (start > end) throw new ArgumentException("Start date must be less than end date");
        if (start > DateTime.UtcNow || end > DateTime.UtcNow) throw new ArgumentException("Date must be in the past");
        var icdRootsEntities = !icdRoots.IsNullOrEmpty()
            ? await _icd10DictionaryService.GetRootsByIcdList(icdRoots)
            : await _icd10DictionaryService.GetRootsWithoutIcdList();

        var filters = new IcdRootsReportFiltersModel
        {
            Start = start,
            End = end,
            IcdRoots = icdRootsEntities
                .Select(x => x.Code)
                .OrderBy(x => x)
                .ToList()
        };
        var inspections = await _db.Inspections
            .Include(x => x.Diagnoses)
            .Include(x => x.Consultations)
            .Where(x => x.Date >= start && x.Date <= end)
            .ToListAsync();
        
        var patientDiseases = new Dictionary<Guid, Dictionary<Guid, int>>();

        foreach (var inspection in inspections)
        {
            if (!patientDiseases.ContainsKey(inspection.PatientId))
            {
                patientDiseases[inspection.PatientId] = new Dictionary<Guid, int>();
            }
            patientDiseases[inspection.PatientId] =
                await GetPatientDiseases(inspection, patientDiseases[inspection.PatientId]);
        }

        var summaryByRoot = new Dictionary<string, int>();
        foreach (var icd in icdRootsEntities)
        {
            summaryByRoot.TryAdd(icd.Code, 0);
        }
        
        var records = new List<IcdRootsReportRecordModel>();
        foreach (var (patientId, diseaseDetails) in patientDiseases)
        {
            var patient = await _db.Patients.FirstOrDefaultAsync(x => x.Id == patientId);
            if (patient == null) continue;

            var record = new IcdRootsReportRecordModel
            {
                PatientName = patient.Name,
                Gender = patient.Gender,
                PatientBirthdate = patient.Birthday,
                VisitsByRoot = new Dictionary<string, int>()
            };

            foreach (var (diseaseId, diseaseCount) in diseaseDetails)
            {
                var disease = await _db.Icd10Roots.FirstOrDefaultAsync(x => x.Id == diseaseId);
                if (disease?.Code == null) continue;
                record.VisitsByRoot.Add(disease.Code, diseaseCount);
                summaryByRoot[disease.Code] += diseaseCount;
            }

            records.Add(record);
        }


        return new IcdRootsReportModel
        {
            Filters = filters,
            Records = records.OrderBy(r => r.PatientName).ToList(),
            SummaryByRoot = summaryByRoot
        };
    }

    private async Task<Dictionary<Guid, int>> GetPatientDiseases(Inspection inspection,
        Dictionary<Guid, int> patientDisease)
    {
        if (inspection.Diagnoses == null) return patientDisease;
        foreach (var diagnosis in inspection.Diagnoses)
        {
            if (!diagnosis.IcdRootId.HasValue) continue;
            var icdRoot = await _db.Icd10Roots.FirstOrDefaultAsync(x => x.Id == diagnosis.IcdRootId);
            if (icdRoot == null) continue;
            if (!patientDisease.ContainsKey(icdRoot.Id))
            {
                patientDisease.TryAdd(icdRoot.Id, 0);
            }
            patientDisease[icdRoot.Id]++;
        }

        return patientDisease;
    }
}