using MisAPI.Enums;
using MisAPI.Models.Api;
using MisAPI.Models.Request;
using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IPatientService
{
    Task<PatientPagedListModel> GetPatients(string? name, Conclusion[]? conclusions, PatientSorting sorting,
        bool scheduledVisits, bool onlyMine, int page, int size, Guid doctorId);

    Task<Guid> CreatePatient(PatientCreateModel patientCreateModel, Guid doctorId);
    Task<Guid> CreateInspection(Guid id, InspectionCreateModel inspectionCreateModel, Guid doctorId);
    Task<InspectionPagedListModel> GetInspections(Guid id, bool grouped, IEnumerable<Guid>? icdRoots, int page, int size, Guid doctorId);
    Task<PatientModel> GetPatientCard(Guid id, Guid doctorId);
    Task<InspectionShortModel> SearchInspections(Guid id, string? request, Guid doctorId);
}