namespace MisAPI.Quartz.Workers.EmailSender;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}