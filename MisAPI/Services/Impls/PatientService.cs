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
        var patients = _db.Patients.AsQueryable().Where(patient => patient.Name.Contains(lowerName)).AsQueryable();

        if (conclusions is { Length: > 0 })
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
        var patientModelList = patients.AsEnumerable()
            .Select(Mapper.EntityPatientToPatientModel)
            .Skip((page - 1) * size)
            .Take(size);

        var totalCount = await patients.CountAsync();
        var totalPages = totalCount / size + (totalCount % size == 0 ? 0 : 1);
        return new PatientPagedListModel(
            patientModelList,
            new PageInfoModel(size, totalPages, page)
        );
    }


    public async Task<Guid> CreateInspection(Guid id, InspectionCreateModel inspectionCreateModel, Guid doctorId)
    {
        var inspection = Mapper.InspectionCreateModelToInspection(inspectionCreateModel);
        inspection.PatientId = id;
        inspection.DoctorId = doctorId;

        var diagnoses = new List<Diagnosis>();
        foreach (var diagnosisCreateModel in inspectionCreateModel.Diagnoses)
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
            diagnoses.Add(diagnosisEntity);
            await _db.Diagnoses.AddAsync(diagnosisEntity);
        }

        var baseInspectionId = inspectionCreateModel.PreviousInspectionId == Guid.Empty || id == Guid.Empty
            ? Guid.Empty
            : await _db.Inspections.Where(i => i.PatientId == id).MaxAsync(i => i.Id);

        var baseInspection = await _db.Inspections.FirstOrDefaultAsync(i => i.Id == baseInspectionId);
        if (baseInspection == null)
            throw new InspectionNotFoundException($"Inspection with id = {baseInspectionId} not found");
        inspection.BaseInspectionId = baseInspection.Id;

        inspection.Diagnoses = diagnoses;
        await _db.Inspections.AddAsync(inspection);
        await _db.SaveChangesAsync();
        return inspection.Id;
    }

    public Task<InspectionPagedListModel> GetInspections(Guid id, bool grouped, IEnumerable<Guid>? icdRoots, int page,
        int size, Guid doctorId)
    {
        // get list of patient inspections
        // if grouped - 
        // Параметр grouped в вашем запросе является флагом (boolean), который указывает, нужно ли группировать медицинские осмотры по цепочке взаимосвязанных осмотров.
        //
        //     Если grouped установлен в true, то осмотры будут отфильтрованы и группированы таким образом, чтобы отобразить связанные медицинские осмотры в цепочках. 
        throw new NotImplementedException();
    }

    public async Task<PatientModel> GetPatientCard(Guid id, Guid doctorId)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        if (patient == null) throw new PatientNotFoundException($"Patient with id = {id} not found");
        return Mapper.EntityPatientToPatientModel(patient);
    }

    public async Task<IEnumerable<InspectionShortModel>> SearchInspections(Guid id, string? request, Guid doctorId)
    {
        var inspections = _db.Inspections.AsQueryable().Where(i => i.PatientId == id);
        var newInspections = inspections.Where(i =>
            i.BaseInspectionId == Guid.Empty ||
            !inspections.Any(i2 => i2.BaseInspectionId == i.Id || i2.PreviousInspectionId == i.Id));
        if (!string.IsNullOrEmpty(request))
        {
            newInspections = newInspections.Where(i => i.Diagnoses != null && i.Diagnoses.Any(d => d.Name.Contains(request)));
        }
        
        return newInspections.AsEnumerable().Select(Mapper.EntityInspectionToInspectionShortModel);
    }
}