using MisAPI.Models.Response;

namespace MisAPI.Services.Interfaces;

public interface IReportService
{
    Task<IcdRootsReportModel> GetIcdRootsReportAsync(DateTime start, DateTime end, IEnumerable<Guid>? icdRoots, Guid doctorId);
}