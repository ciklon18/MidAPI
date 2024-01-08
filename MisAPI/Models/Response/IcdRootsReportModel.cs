using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public class IcdRootsReportModel
{
    public IcdRootsReportFiltersModel Filters { get; set; } = null!;
    public ICollection<IcdRootsReportRecordModel> Records { get; set; } = null!;
    public Dictionary<string, int> SummaryByRoot { get; set; } = null!;
}