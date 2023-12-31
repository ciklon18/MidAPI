using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record PatientPagedListModel(IEnumerable<PatientModel> Patients
    // , PageInfoModel Pagination
    )
{
    public PatientPagedListModel() : this(Array.Empty<PatientModel>()
        // , new PageInfoModel()
        )
    {
    }
}