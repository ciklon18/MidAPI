using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Enums;
using MisAPI.Exceptions;
using MisAPI.Mappers;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _db;
    private readonly IIcd10DictionaryService _icd10DictionaryService;

    public PatientService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService)
    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
    }

    public async Task<Guid> CreatePatient(PatientCreateModel patientCreateModel, Guid doctorId)
    {
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Birthday = patientCreateModel.Birthday,
            CreateTime = DateTime.Now,
            Gender = patientCreateModel.Gender,
            Id = patientId,
            Name = patientCreateModel.Name,
            DoctorId = doctorId
        };
        await _db.Patients.AddAsync(patient);
        await _db.SaveChangesAsync();

        return patientId;
    }

    public async Task<PatientPagedListModel> GetPatients(string? name, Conclusion[]? conclusions,
        PatientSorting sorting,
        bool scheduledVisits,
        bool onlyMine,
        int page, int size, Guid doctorId)
    {
        var lowerName = name?.ToLower() ?? string.Empty;
        
        var patients = _db.Patients
            .Where(patient => patient.Name.Contains(lowerName));

        if (conclusions?.Length > 0)
        {
            patients = patients.Where(patient =>
                    patient.Inspections.Any(inspection => conclusions.Contains(inspection.Conclusion)));
        }

        if (scheduledVisits)
        {
            patients = patients.Where(
                    patient => patient.Inspections.Any(inspection => inspection.NextVisitDate != null));
        }

        if (onlyMine)
        {
            patients = patients.Where(patient => patient.DoctorId == doctorId);
        }

        patients = sorting switch
        {
            PatientSorting.NameAsc => patients.OrderBy(patient => patient.Name),
            PatientSorting.NameDesc => patients.OrderByDescending(patient => patient.Name),
            PatientSorting.CreateAsc => patients.OrderBy(patient => patient.CreateTime),
            PatientSorting.CreateDesc => patients.OrderByDescending(patient => patient.CreateTime),
            PatientSorting.InspectionAsc => patients.OrderBy(patient =>
                patient.Inspections.Max(inspection => inspection.Date)),
            PatientSorting.InspectionDesc => patients.OrderByDescending(patient =>
                patient.Inspections.Max(inspection => inspection.Date)),
            _ => patients
        };
        var totalCount = await patients.CountAsync();
    
        if (page > totalCount / size && totalCount > 0)
        {
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");
        }

        var totalPages = (int)Math.Ceiling((double)totalCount / size);
        var patientModelList = patients
            .Skip((page - 1) * size)
            .Take(size)
            .Include(patient => patient.Inspections)
            .Select(Mapper.EntityPatientToPatientModel)
            .ToList();

        return new PatientPagedListModel(
            patientModelList,
            new PageInfoModel(size, totalPages, page)
        );
    }


    public async Task<Guid> CreateInspection(Guid id, InspectionCreateModel inspectionCreateModel, Guid doctorId)
    {
        var inspection = Mapper.InspectionCreateModelToInspection(inspectionCreateModel);
        
        
        
        if (inspection.Date > DateTime.Now)
            throw new InvalidValueForAttributeDateException("Date of inspection cannot be in the future");
        if (inspection.PreviousInspectionId != Guid.Empty)
        {
            var previousInspection = await _db.Inspections.FirstOrDefaultAsync(i => i.Id == inspection.PreviousInspectionId);
            if (previousInspection == null)
                throw new InspectionNotFoundException(
                    $"Inspection with id = {inspection.PreviousInspectionId} not found");
            if (previousInspection.Date > inspection.Date)
                throw new InvalidValueForAttributeDateException("Date of inspection cannot be earlier than date of previous inspection");
        }
        switch (inspection.Conclusion)
        {
            case Conclusion.Disease when inspection.NextVisitDate == null:
                throw new InvalidValueForAttributeDateException("Next visit date cannot be null if conclusion is disease");
            case Conclusion.Death when inspection.DeathDate == null:
                throw new InvalidValueForAttributeDateException("Death date cannot be null if conclusion is death");
            case Conclusion.Recovery when inspection.DeathDate != null:
                throw new InvalidValueForAttributeDateException("Death date must be null if conclusion is recovery");
            case Conclusion.Recovery when inspection.NextVisitDate != null:
                throw new InvalidValueForAttributeDateException("Next visit date must be null if conclusion is recovery");
        }
        if (inspectionCreateModel.Diagnoses == null || !inspectionCreateModel.Diagnoses.Any())
            throw new InvalidValueForAttributeDiagnosesException("Diagnoses cannot be null or empty");
        if (inspectionCreateModel.Diagnoses.Count(d => d.Type == DiagnosisType.Main) != 1)
            throw new InvalidValueForAttributeDiagnosesException("Inspection must have one main diagnosis");

        
        
        
        var diagnoses = await Task.WhenAll(inspectionCreateModel.Diagnoses.Select(async diagnosisCreateModel =>
        {
            var diagnosisEntity = Mapper.DiagnosisCreateModelToDiagnosis(diagnosisCreateModel);
            var diagnosisFromDb =
                await _icd10DictionaryService.GetIcd10DiagnosisAsync(diagnosisCreateModel.IcdDiagnosisId);

            if (diagnosisFromDb == null)
                throw new DiagnosisNotFoundException(
                    $"Diagnosis with id = {diagnosisCreateModel.IcdDiagnosisId} not found");

            diagnosisEntity.Code = diagnosisFromDb.Code;
            diagnosisEntity.Name = diagnosisFromDb.Name;
            diagnosisEntity.InspectionId = inspection.Id;

            return diagnosisEntity;
        }));
        await _db.Diagnoses.AddRangeAsync(diagnoses);


        var baseInspectionId = inspectionCreateModel.PreviousInspectionId == Guid.Empty || id == Guid.Empty
            ? Guid.Empty
            : await _db.Inspections
                .Where(i => i.PatientId == id)
                .OrderByDescending(i => i.Id)
                .Select(i => i.Id)
                .FirstOrDefaultAsync();


        var baseInspection = await _db.Inspections.FirstOrDefaultAsync(i => i.Id == baseInspectionId);
        if (baseInspection == null)
            throw new InspectionNotFoundException($"Inspection with id = {baseInspectionId} not found");
        
        inspection.BaseInspectionId = baseInspection.Id;
        inspection.Diagnoses = diagnoses;
        inspection.PatientId = id;
        inspection.DoctorId = doctorId;
        
        await _db.Inspections.AddAsync(inspection);
        await _db.SaveChangesAsync();
        
        return inspection.Id;
    }

    public async Task<InspectionPagedListModel> GetInspections(Guid id, bool grouped, IEnumerable<Guid>? icdRoots,
        int page, int size, Guid doctorId)
    {
        var enumerable = icdRoots != null ? icdRoots.ToList() : new List<Guid>();
        await CheckAreIcdRootsExist(enumerable);
        var inspections = _db.Inspections.AsQueryable().Where(i => i.PatientId == id);
        
        if (grouped)
        {
            inspections = inspections.Where(i => i.PreviousInspectionId == null || i.PreviousInspectionId == Guid.Empty);
        }

        var inspectionsList = inspections
            .Include(inspection => inspection.Diagnoses)
            .AsEnumerable()
            .Select(inspection =>
            {
                var diagnosis = inspection.Diagnoses?.FirstOrDefault(d => d.Type == DiagnosisType.Main);
                var diagnosisModel = diagnosis != null ? Mapper.DiagnosisToDiagnosisModel(diagnosis) : null;
                return Mapper.EntityInspectionToInspectionPreviewModel(inspection, diagnosisModel);
            });

        if (icdRoots != null)
        {
            inspectionsList = inspectionsList.Where(i =>
                i.Diagnosis != null && enumerable.Contains(i.Diagnosis.Id));
        }

        inspectionsList = inspectionsList.ToList();
        var count = inspectionsList.Count();
        if (page > count / size && !inspectionsList.Any())
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");
        
        var totalPages = (int)Math.Ceiling((double)count / size);

        inspectionsList = inspectionsList
            .Skip((page - 1) * size)
            .Take(size);
        return new InspectionPagedListModel(inspectionsList, new PageInfoModel(size, totalPages, page));
    }

    
    
    
    
    private async Task CheckAreIcdRootsExist(IEnumerable<Guid>? icdRoots)
    {
        if (icdRoots == null) return;
        var icdRootsList = _db.Mkb10Roots.AsQueryable();
        foreach (var icdRoot in icdRoots)
        {
            if (!await icdRootsList.AnyAsync(i => i.Id == icdRoot))
                throw new IcdRootNotFoundException($"Icd root with id = {icdRoot} not found");
        }
    }

    public async Task<PatientModel> GetPatientCard(Guid id, Guid doctorId)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new PatientNotFoundException($"Patient with id = {id} not found");
        return Mapper.EntityPatientToPatientModel(patient);
    }

    public Task<IEnumerable<InspectionShortModel>> SearchInspections(Guid id, string? request, Guid doctorId)
    {
        var inspections = _db.Inspections.AsQueryable().Where(i => i.PatientId == id);
        var newInspections = inspections.Where(i =>
            i.BaseInspectionId == Guid.Empty ||
            inspections.Any(i2 => i2.BaseInspectionId == i.Id || i2.PreviousInspectionId == i.Id));
        if (!string.IsNullOrEmpty(request))
        {
            newInspections =
                newInspections.Where(i => i.Diagnoses != null && i.Diagnoses.Any(d => d.Name.Contains(request)));
        }

        return Task.FromResult(newInspections.AsEnumerable().Select(Mapper.EntityInspectionToInspectionShortModel));
    }
}