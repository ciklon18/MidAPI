using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record SpecialtiesPagedListModel(List<SpecialityModel> Specialities, PageInfoModel Pagination)
{
    public SpecialtiesPagedListModel() : this(new List<SpecialityModel>(), new PageInfoModel())
    {
    }
}