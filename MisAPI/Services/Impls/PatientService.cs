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
    private readonly IInspectionService _inspectionService;

    public PatientService(ApplicationDbContext db, IIcd10DictionaryService icd10DictionaryService,
        IJwtService jwtService, IInspectionService inspectionService)
    {
        _db = db;
        _icd10DictionaryService = icd10DictionaryService;
        _jwtService = jwtService;
        _inspectionService = inspectionService;
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


        await ValidateDeathConclusion(patient.Inspections);
        await ValidateInspectionDate(inspection.Date);
        await ValidatePreviousInspectionAsync(inspection.PreviousInspectionId, patient.Id);

        await _inspectionService.ValidateConclusionAndDates(Mapper
            .MapInspectionCreateModelToConclusionAndDateValidationModel(inspectionCreateModel));
        await _inspectionService.ValidateDiagnoses(inspectionCreateModel.Diagnoses);

        var diagnoses = await _inspectionService.MapDiagnosesAsync(inspection.Id, inspectionCreateModel.Diagnoses);

        var consultations =
            await CreateConsultationsAsync(inspectionCreateModel.Consultations, inspection.Id, doctorId);

        var (baseInspectionId, previousInspectionId) =
            await GetBaseInspectionAsync(inspectionCreateModel, patient.Inspections, inspection);

        
        var inspectionEntity = GetUpdatedInspectionEntity(inspection, doctorId, patient.Id, diagnoses, consultations,
            baseInspectionId, previousInspectionId);

        await _db.Inspections.AddAsync(inspectionEntity);

        await _db.Diagnoses.AddRangeAsync(diagnoses);
        await _db.Consultations.AddRangeAsync(consultations);

        await _db.SaveChangesAsync();
        return inspection.Id;
    }

    private static Inspection GetUpdatedInspectionEntity(Inspection inspection, Guid doctorId, Guid patientId,
        ICollection<Diagnosis> diagnoses, ICollection<Consultation> consultations, Guid? baseInspectionId, Guid? previousInspectionId)
    {
        inspection.PatientId = patientId;
        inspection.DoctorId = doctorId;
        inspection.CreateTime = DateTime.UtcNow;
        inspection.Date = inspection.Date.ToUniversalTime();
        inspection.NextVisitDate =
            inspection.NextVisitDate?.ToUniversalTime();
        inspection.DeathDate = inspection.DeathDate?.ToUniversalTime();
        inspection.Diagnoses = diagnoses;
        inspection.Consultations = consultations;
        if (baseInspectionId != null) inspection.BaseInspectionId = baseInspectionId.Value;
        if (previousInspectionId != null) inspection.PreviousInspectionId = previousInspectionId.Value;

        return inspection;
    }


    private static Task ValidateDeathConclusion(IEnumerable<Inspection> inspections)
    {
        if (inspections.Any(i => i.Conclusion == Conclusion.Death))
            throw new InvalidValueForAttributeConclusionException(
                "Inspection cannot be created because the patient has a death conclusion");
        return Task.CompletedTask;
    }

    private static Task ValidateInspectionDate(DateTime inspectionDate)
    {
        if (inspectionDate > DateTime.Now)
            throw new InvalidValueForAttributeDateException("Date of inspection cannot be in the future");
        return Task.CompletedTask;
    }

    private async Task ValidatePreviousInspectionAsync(Guid? previousInspectionId, Guid currentPatientId)
    {
        if (previousInspectionId != Guid.Empty)
        {
            var previousInspection = await _db.Inspections.FirstOrDefaultAsync(i => i.Id == previousInspectionId);

            if (previousInspection == null)
                throw new InspectionNotFoundException($"Inspection with id = {previousInspectionId} not found");

            if (previousInspection.PatientId != currentPatientId)
                throw new PatientNotFoundException("Previous inspection must belong to the same patient");

            if (await _db.Inspections.AnyAsync(i => i.PreviousInspectionId == previousInspectionId))
                throw new InspectionIsNotRootException(
                    $"Inspection with id = {previousInspectionId} cannot be more than one root");

            if (previousInspection.Date > DateTime.Now)
                throw new InvalidValueForAttributeDateException("Date of previous inspection cannot be in the future");

            if (previousInspection.Date > DateTime.Now)
                throw new InvalidValueForAttributeDateException(
                    "Date of inspection cannot be earlier than date of previous inspection");
        }
    }

    private async Task<List<Consultation>> CreateConsultationsAsync(
        IEnumerable<ConsultationCreateModel>? consultationCreateModels, Guid inspectionId, Guid doctorId)
    {
        var consultations = new List<Consultation>();

        if (consultationCreateModels == null) return consultations;
        foreach (var consultationCreateModel in consultationCreateModels)
        {
            await ValidateSpecialtyExistsAsync(consultationCreateModel.SpecialityId);

            var consultation =
                Mapper.MapConsultationCreateModelToConsultation(consultationCreateModel, inspectionId, doctorId);
            consultations.Add(consultation);
        }

        return consultations;
    }

    private async Task ValidateSpecialtyExistsAsync(Guid specialityId)
    {
        if (!await _db.Specialities.AnyAsync(s => s.Id == specialityId))
            throw new SpecialityNotFoundException($"Specialty with id = {specialityId} not found");
    }

    private async Task<(Guid? baseInspectionId, Guid? previousInspectionId)> GetBaseInspectionAsync(
        InspectionCreateModel inspectionCreateModel, IEnumerable<Inspection> patientInspections, Inspection inspection)
    {
        Guid? baseInspectionId = null;
        Guid? previousInspectionId = null;

        if (inspectionCreateModel.PreviousInspectionId != Guid.Empty)
        {
            var baseInspection = await FetchBaseInspectionAsync(inspectionCreateModel.PreviousInspectionId);

            ValidateBaseInspection(baseInspection, inspection, patientInspections);

            baseInspectionId = baseInspection.Id;
            previousInspectionId = inspectionCreateModel.PreviousInspectionId;
        }

        return (baseInspectionId, previousInspectionId);
    }

    private async Task<Inspection> FetchBaseInspectionAsync(Guid previousInspectionId)
    {
        var baseInspection = await _db.Inspections
            .Where(i => i.Id == previousInspectionId)
            .Join(
                _db.Inspections,
                firstInspection => firstInspection.BaseInspectionId ?? firstInspection.Id,
                secondInspection => secondInspection.Id,
                (firstInspection, secondInspection) => secondInspection
            )
            .FirstOrDefaultAsync();

        if (baseInspection == null)
            throw new InspectionNotFoundException($"Inspection with id = {previousInspectionId} not found");

        return baseInspection;
    }

    private void ValidateBaseInspection(Inspection baseInspection, Inspection inspection,
        IEnumerable<Inspection> patientInspections)
    {
        if (baseInspection.Date > inspection.Date)
            throw new InvalidValueForAttributeDateException(
                "Date of inspection cannot be earlier than date of base inspection");

        if (patientInspections.Any(i => i.Conclusion == Conclusion.Recovery && i.BaseInspectionId == baseInspection.Id))
            throw new InvalidValueForAttributeConclusionException(
                "Inspection cannot be created because the patient has a recovery conclusion");
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
            previewInspectionsList.Add(
                Mapper.MapEntityInspectionToInspectionPreviewModel(inspection, diagnosisModel, hasChain, hasNested));
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