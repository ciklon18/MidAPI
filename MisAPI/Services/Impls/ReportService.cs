using MisAPI.Models.Response;
using MisAPI.Services.Interfaces;

namespace MisAPI.Services.Impls;

public class ReportService : IReportService
{
    public Task<IcdRootsReportModel> GetIcdRootsReportAsync(DateTime start, DateTime end, IEnumerable<Guid>? icdRoots, Guid doctorId)
    {
        throw new NotImplementedException();
    }
}