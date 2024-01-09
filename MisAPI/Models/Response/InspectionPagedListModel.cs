using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record InspectionPagedListModel(ICollection<InspectionPreviewModel> Inspections, PageInfoModel Pagination)
{
    public InspectionPagedListModel() : this(new List<InspectionPreviewModel>(), new PageInfoModel())
    {
    }
}