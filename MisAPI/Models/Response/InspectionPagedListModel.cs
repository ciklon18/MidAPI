using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public record InspectionPagedListModel(IEnumerable<InspectionPreviewModel> Inspections, PageInfoModel Pagination)
{
    public InspectionPagedListModel() : this(Enumerable.Empty<InspectionPreviewModel>(), new PageInfoModel())
    {
    }
}