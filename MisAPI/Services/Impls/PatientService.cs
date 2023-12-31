using MisAPI.Enums;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class PatientService : IPatientService
{
    public Task<PatientPagedListModel> GetPatients(string? name, Conclusion[]? conclusions, PatientSorting sorting,
        bool scheduledVisits, bool onlyMine,
        int page, int size, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreatePatient(PatientCreateModel patientCreateModel, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> CreateInspection(Guid id, InspectionCreateModel inspectionCreateModel, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionPagedListModel> GetInspections(Guid id, bool grouped, IEnumerable<Guid>? icdRoots, int page, int size, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<PatientModel> GetPatientCard(Guid id, Guid doctorId)
    {
        throw new NotImplementedException();
    }

    public Task<InspectionShortModel> SearchInspections(Guid id, string? request, Guid doctorId)
    {
        throw new NotImplementedException();
    }
}