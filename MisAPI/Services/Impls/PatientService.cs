using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    private readonly IJwtService _jwtService;

    public PatientService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService,
        IJwtService jwtService)
    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
        _jwtService = jwtService;
    }


    public async Task<Guid> CreatePatient(PatientCreateModel patientCreateModel, Guid doctorId)
    {
        await _jwtService.CheckIsRefreshTokenValidAsync(doctorId);
        var patientId = Guid.NewGuid();
        var patient = new Patient
        {
            Birthday = patientCreateModel.Birthday.ToUniversalTime(),
            CreateTime = DateTime.UtcNow,
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
        var totalPages = (int)Math.Ceiling((double)await patients.CountAsync() / size);

        if (page > totalPages)
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");

        var patientModelList = patients
            .Skip((page - 1) * size)
            .Take(size)
            .Include(patient => patient.Inspections)
            .Select(Mapper.MapEntityPatientToPatientModel)
            .ToList();

        return new PatientPagedListModel(
            patientModelList,
            new PageInfoModel(size, totalPages, page)
        );
    }


    public async Task<Guid> CreateInspection(Guid id, InspectionCreateModel inspectionCreateModel, Guid doctorId)
    {
        var patient = await _db.Patients
            .Include(patient => patient.Inspections)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new PatientNotFoundException($"Patient with id = {id} not found");
        var inspection = Mapper.MapInspectionCreateModelToInspection(inspectionCreateModel);

        if (patient.Inspections.Any(i => i.Conclusion == Conclusion.Death))
            throw new InvalidValueForAttributeConclusionException(
                "Inspection cannot be created because patient has death conclusion");

        if (inspection.Date > DateTime.Now)
            throw new InvalidValueForAttributeDateException("Date of inspection cannot be in the future");
        if (inspection.PreviousInspectionId != Guid.Empty)
        {
            var previousInspection =
                await _db.Inspections.FirstOrDefaultAsync(i => i.Id == inspection.PreviousInspectionId);
            if (previousInspection == null)
                throw new InspectionNotFoundException(
                    $"Inspection with id = {inspection.PreviousInspectionId} not found");
            if (previousInspection.PatientId != id)
                throw new PatientNotFoundException("Previous inspection must belong to the same patient");
            if (await _db.Inspections.AnyAsync(i => i.PreviousInspectionId == inspection.PreviousInspectionId))
                throw new InspectionIsNotRootException(
                    $"Inspection with id = {inspection.PreviousInspectionId} cannot be more than one root");
            if (previousInspection.Date > inspection.Date)
                throw new InvalidValueForAttributeDateException(
                    "Date of inspection cannot be earlier than date of previous inspection");
        }

        switch (inspection.Conclusion)
        {
            case Conclusion.Disease when inspection.NextVisitDate == null:
                throw new InvalidValueForAttributeDateException(
                    "Next visit date cannot be null if conclusion is disease");
            case Conclusion.Death when inspection.DeathDate == null:
                throw new InvalidValueForAttributeDateException("Death date cannot be null if conclusion is death");
            case Conclusion.Recovery when inspection.DeathDate != null:
                throw new InvalidValueForAttributeDateException("Death date must be null if conclusion is recovery");
            case Conclusion.Recovery when inspection.NextVisitDate != null:
                throw new InvalidValueForAttributeDateException(
                    "Next visit date must be null if conclusion is recovery");
            case Conclusion.Disease when inspection.DeathDate != null:
                throw new InvalidValueForAttributeDateException("Death date must be null if conclusion is disease");
            case Conclusion.Recovery when inspection.DeathDate != null:
                throw new InvalidValueForAttributeDateException("Death date must be null if conclusion is recovery");
        }

        if (inspectionCreateModel.Diagnoses == null || !inspectionCreateModel.Diagnoses.Any())
            throw new InvalidValueForAttributeDiagnosesException("Diagnoses cannot be null or empty");
        if (inspectionCreateModel.Diagnoses.Count(d => d.Type == DiagnosisType.Main) != 1)
            throw new InvalidValueForAttributeDiagnosesException("Inspection must have one main diagnosis");

        var diagnoses = new List<Diagnosis>();
        foreach (var diagnosisCreateModel in inspectionCreateModel.Diagnoses)
        {
            var diagnosisEntity = Mapper.MapDiagnosisCreateModelToDiagnosis(diagnosisCreateModel);
            var diagnosisFromDb =
                await _icd10DictionaryService.GetIcd10DiagnosisAsync(diagnosisCreateModel.IcdDiagnosisId);
            if (diagnosisFromDb == null)
                throw new DiagnosisNotFoundException(
                    $"Diagnosis with id = {diagnosisCreateModel.IcdDiagnosisId} not found");

            diagnosisEntity.Code = diagnosisFromDb.Code;
            diagnosisEntity.Name = diagnosisFromDb.Name;
            diagnosisEntity.InspectionId = inspection.Id;

            diagnoses.Add(diagnosisEntity);
        }

        var baseInspectionId = inspectionCreateModel.PreviousInspectionId == Guid.Empty || id == Guid.Empty
            ? Guid.Empty
            : await _db.Inspections
                .Where(i => i.Id == inspectionCreateModel.PreviousInspectionId)
                .Select(i => i.BaseInspectionId)
                .FirstOrDefaultAsync();

        if (baseInspectionId != null && baseInspectionId != Guid.Empty)
        {
            var baseInspection = await _db.Inspections.FirstOrDefaultAsync(i => i.Id == baseInspectionId);
            if (baseInspection == null)
                throw new InspectionNotFoundException($"Inspection with id = {baseInspectionId} not found");
            if (baseInspection.Date > inspection.Date)
                throw new InvalidValueForAttributeDateException(
                    "Date of inspection cannot be earlier than date of base inspection");
            if (patient.Inspections.Any(i => i is { Conclusion: Conclusion.Recovery, BaseInspectionId: not null }))
                throw new InvalidValueForAttributeConclusionException(
                    "Inspection cannot be created because patient has recovery conclusion");
            inspection.BaseInspectionId = baseInspection.Id;
        }


        inspection.PatientId = patient.Id;
        inspection.DoctorId = doctorId;
        inspection.CreateTime = DateTime.UtcNow;
        inspection.Date = inspection.Date.ToUniversalTime();
        inspection.NextVisitDate =
            inspection.NextVisitDate?.ToUniversalTime();
        inspection.DeathDate = inspection.DeathDate?.ToUniversalTime();

        inspection.PreviousInspectionId = inspectionCreateModel.PreviousInspectionId == Guid.Empty
            ? null
            : inspectionCreateModel.PreviousInspectionId;

        var consultations = new List<Consultation>();

        if (inspectionCreateModel.Consultations != null)
        {
            foreach (var consultationCreateModel in inspectionCreateModel.Consultations)
            {
                if (!await _db.Specialities.AnyAsync(s => s.Id == consultationCreateModel.SpecialityId))
                    throw new SpecialityNotFoundException(
                        $"Specialty with id = {consultationCreateModel.SpecialityId} not found");
                var consultation = Mapper.MapConsultationCreateModelToConsultation(consultationCreateModel,
                    inspection.Id, doctorId);
                consultations.Add(consultation);
            }
        }


        inspection.Diagnoses = diagnoses;
        inspection.Consultations = consultations;

        await _db.Inspections.AddAsync(inspection);

        await _db.Diagnoses.AddRangeAsync(diagnoses);
        await _db.Consultations.AddRangeAsync(consultations);

        await _db.SaveChangesAsync();
        return inspection.Id;
    }

    public async Task<InspectionPagedListModel> GetInspections(Guid id, bool grouped, ICollection<Guid>? icdRoots,
        int page, int size, Guid doctorId)
    {
        await _icd10DictionaryService.CheckAreIcdRootsExist(icdRoots);

        var inspections = _db.Inspections
            .Where(i => i.PatientId == id)
            .Include(i => i.Diagnoses)
            .Include(i => i.Patient)
            .Include(i => i.Doctor)
            .AsQueryable();

        if (grouped)
        {
            inspections =
                inspections.Where(i => i.PreviousInspectionId == null || i.PreviousInspectionId == Guid.Empty);
        }

        var inspectionsList = await inspections.ToListAsync();
        var previewInspectionsList = new List<InspectionPreviewModel>();
        var isIcdRootsNullOrEmpty = icdRoots.IsNullOrEmpty();
        foreach (var inspection in inspectionsList)
        {
            var diagnosis = inspection.Diagnoses?.FirstOrDefault(d => d.Type == DiagnosisType.Main);
            DiagnosisModel? diagnosisModel = null;
            if (diagnosis != null && (isIcdRootsNullOrEmpty || await _db.Icd10
                    .AnyAsync(d =>
                        d.IdGuid == diagnosis.IcdDiagnosisId 
                        && !isIcdRootsNullOrEmpty
                        && icdRoots.Contains((Guid)d.IdGuid))))
            {
                diagnosisModel = Mapper.MapDiagnosisToDiagnosisModel(diagnosis);
            }

            var hasChain = inspection.BaseInspectionId == null;
            var hasNested = await _db.Inspections.AnyAsync(i => i.PreviousInspectionId == inspection.Id);
            previewInspectionsList.Add(Mapper.MapEntityInspectionToInspectionPreviewModel(inspection, diagnosisModel, hasChain, hasNested));
        }


        var totalPages = (int)Math.Ceiling((double)previewInspectionsList.Count / size);

        if (page > totalPages)
            throw new InvalidValueForAttributePageException("Invalid value for attribute page");

        previewInspectionsList = previewInspectionsList
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        return new InspectionPagedListModel(previewInspectionsList, new PageInfoModel(size, totalPages, page));
    }


    public async Task<PatientModel> GetPatientCard(Guid id, Guid doctorId)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new PatientNotFoundException($"Patient with id = {id} not found");
        return Mapper.MapEntityPatientToPatientModel(patient);
    }

    public Task<IEnumerable<InspectionShortModel>> SearchInspections(Guid id, string? request, Guid doctorId)
    {
        var inspections = _db.Inspections
            .Where(i => i.PatientId == id)
            .Include(i => i.Diagnoses)
            .AsQueryable();
        
        var newInspections = inspections
            .Where(i =>
                i.BaseInspectionId == Guid.Empty ||
                !inspections.Any(i2 => i2.BaseInspectionId == i.Id || i2.PreviousInspectionId == i.Id));
        if (!string.IsNullOrEmpty(request))
        {
            var requestLower = request.ToLower();
            newInspections = newInspections
                .Where(i =>
                    i.Diagnoses != null
                    && i.Diagnoses.Any(d => d.Name.ToLower().Contains(requestLower)));
        }

        var result = newInspections
            .AsEnumerable()
            .Select(Mapper.MapEntityInspectionToInspectionShortModel);
        return Task.FromResult(result);
    }
}