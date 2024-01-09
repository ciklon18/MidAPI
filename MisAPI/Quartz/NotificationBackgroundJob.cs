using MisAPI.Worker;
using Quartz;

namespace MisAPI.Quartz;


[DisallowConcurrentExecution] // блокирует запуск задачи, если предыдущая еще не завершилась
public class NotificationBackgroundJob : IJob
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NotificationBackgroundJob> _logger;

    public NotificationBackgroundJob(IEmailSender emailSender, ILogger<NotificationBackgroundJob> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var email = "example@gmail.com";
            var subject = "NotificationBackgroundJob executed";
            var message = "NotificationBackgroundJob executed";
            _logger.LogTrace("NotificationBackgroundJob executed");
            await _emailSender.SendEmailAsync(email, subject, message);
            _logger.LogTrace("Email was sent");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
        }
    }
}