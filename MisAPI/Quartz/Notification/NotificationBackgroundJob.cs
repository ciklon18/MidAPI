using MisAPI.Data;
using MisAPI.Quartz.Workers.EmailSender;
using Quartz;

namespace MisAPI.Quartz.Notification;

[DisallowConcurrentExecution] // блокирует запуск задачи, если предыдущая еще не завершилась
public class NotificationBackgroundJob : IJob
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationBackgroundJob> _logger;
    private readonly ApplicationDbContext _db;

    private const string Subject = "Reminder about visit";
    private const string Message = "Patient didn't visit doctor";
    private const int MaxRetryAttempts = 3;

    public NotificationBackgroundJob(IEmailSender emailSender, ILogger<NotificationBackgroundJob> logger,
        ApplicationDbContext db)
    {
        _emailSender = emailSender;
        _logger = logger;
        _db = db;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var notifications = _db.Notifications
                .Where(n => n.IsSent == false && n.ScheduledDate.AddHours(1) <= DateTime.UtcNow);

            _logger.LogInformation("Sending notifications, where notifications count = {count}", notifications.Count());

            foreach (var notification in notifications)
            {
                var currentAttempt = 1;
                var success = false;

                while (!success && currentAttempt <= MaxRetryAttempts)
                {
                    try
                    {
                        _logger.LogInformation("Sending notification to {email}", notification.Email);
                        await _emailSender.SendEmailAsync(notification.Email, Subject, Message);
                        notification.IsSent = true;
                        success = true;
                        _logger.LogInformation("Notification sent to {email}", notification.Email);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Attempt {currentAttempt}: Error sending email to {email}. {error}",
                            currentAttempt, notification.Email, e.Message);
                    }

                    currentAttempt++;
                }

                if (!success)
                {
                    _logger.LogError(
                        "Notification to {email} failed after {maxRetryAttempts} attempts. " +
                        "Current status: IsSent={isSent}", notification.Email, MaxRetryAttempts, notification.IsSent);
                }
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Notifications sent");
        }
        catch (Exception e)
        {
            _logger.LogError("Error while sending notifications. {error}", e.Message);
            await transaction.RollbackAsync();
        }
    }
}