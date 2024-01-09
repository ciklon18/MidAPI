using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record PatientPagedListModel(ICollection<PatientModel> Patients, PageInfoModel Pagination)
{
    public PatientPagedListModel() : this(new List<PatientModel>(), new PageInfoModel()
    )
    {
    }
}