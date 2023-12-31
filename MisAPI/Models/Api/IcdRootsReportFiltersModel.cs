namespace MisAPI.Models.Api;

public class IcdRootsReportFiltersModel
{

    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public IEnumerable<string> IcdRoots { get; set; } = null!;
}