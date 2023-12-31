using MisAPI.Models.Api;

namespace MisAPI.Models.Response;

public class IcdRootsReportModel
{
    public IEnumerable<IcdRootsReportFiltersModel> Filters { get; set; } = null!;
    public IEnumerable<IcdRootsReportRecordModel> Records { get; set; } = null!;
    public Dictionary<string, int> SummaryByRoot { get; set; } = null!;
}