using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using MisAPI.Entities;
using MisAPI.Enums;
using MisAPI.Mappers;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class PatientService : IPatientService
{
    private readonly ApplicationDbContext _db;

    public PatientService(ApplicationDbContext db)
    {
        _db = db;
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
            patients = patients.Where(patient => patient.Inspections.Any(inspection => inspection.NextVisitDate != null));
        }

        if (onlyMine)
        {
            patients = patients.Where(patient => patient.DoctorId == doctorId);
        }
        // switch (sorting)
        // {
        //     case PatientSorting.NameAsc:
        //         patientModelList = patientModelList.OrderBy(patient => patient.Name);
        //         break;
        //     case PatientSorting.NameDesc:
        //         patientModelList = patientModelList.OrderByDescending(patient => patient.Name);
        //         break;
        //     case PatientSorting.CreateAsc:
        //         patientModelList = patientModelList.OrderBy(patient => patient.CreateDate);
        //         break;
        //     case PatientSorting.CreateDesc:
        //         patientModelList = patientModelList.OrderByDescending(patient => patient.CreateDate);
        //         break;
        //     case PatientSorting.InspectionAsc:
        //         patientModelList = patientModelList.OrderBy(patient => patient.Inspe);
        //         break;
        //     case PatientSorting.InspectionDesc:
        //         patientModelList = patientModelList.OrderByDescending(patient => patient.LastInspectionDate);
        //         break;
        // }
        var patientModelList = patients.AsEnumerable()
            .Select(Mapper.EntityPatientToPatientModel)
            .Skip((page - 1) * size)
            .Take(size);
        // sort

        var totalCount = await patients.CountAsync();
        var totalPages = totalCount / size + (totalCount % size == 0 ? 0 : 1);
        return new PatientPagedListModel(
            patientModelList,
            new PageInfoModel(size, totalPages, page)
            );
    }


    public Task<Guid> CreateInspection(Guid id, InspectionCreateModel inspectionCreateModel, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionPagedListModel> GetInspections(Guid id, bool grouped, IEnumerable<Guid>? icdRoots, int page,
        int size, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public async Task<PatientModel> GetPatientCard(Guid id, Guid doctorId)
    {
        var patient = await _db.Patients.FirstOrDefaultAsync(p => p.Id == id);
        // if (patient == null) throw new PatientNotFoundException("Patient not found");
        return Mapper.EntityPatientToPatientModel(patient);
    }

    public Task<InspectionShortModel> SearchInspections(Guid id, string? request, Guid doctorId)
    {
        throw new NotImplementedException();
    }
}