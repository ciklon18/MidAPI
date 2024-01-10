using Microsoft.EntityFrameworkCore;
using MisAPI.Data;
using Quartz;

namespace MisAPI.Quartz.DataCollecting;

[DisallowConcurrentExecution]
public class DataCollectingJob : IJob
{
    private readonly ILogger<DataCollectingJob> _logger;
    private readonly ApplicationDbContext _db;

    public DataCollectingJob(ILogger<DataCollectingJob> logger, ApplicationDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var inspections = _db.Inspections
                .Include(inspection => inspection.Doctor)
                .Where(i => !i.IsChecked && i.NextVisitDate != null
                                            && i.NextVisitDate.Value.AddHours(1) < DateTime.UtcNow)
                .Select(i => new
                {
                    Inspection = i,
                    HasNested = _db.Inspections.Any(inner => inner.PreviousInspectionId == i.Id)
                });

            foreach (var inspection in inspections)
            {
                if (!inspection.HasNested)
                {
                    _db.Notifications.Add(
                        new Entities.Notification(
                            inspection.Inspection.Doctor.Email,
                            inspection.Inspection.NextVisitDate ?? DateTime.UtcNow
                        )
                    );
                }
                inspection.Inspection.IsChecked = true;
            }

            _db.Inspections.UpdateRange(inspections.Select(i => i.Inspection));
            await _db.Notifications.AddRangeAsync();

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while collecting and saving data");
            await transaction.RollbackAsync();
        }
    }
}