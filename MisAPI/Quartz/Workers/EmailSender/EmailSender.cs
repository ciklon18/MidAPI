using System.Net.Mail;

namespace MisAPI.Quartz.Workers.EmailSender;

public class EmailSender : IEmailSender
{
    private readonly ILogger<EmailSender> _logger;
    
    public EmailSender(ILogger<EmailSender> logger)
    {
        _logger = logger;
    }
    
    public Task SendEmailAsync(string email, string subject, string message)
    {
        const string from = "mid_reminder@hits.com";
        const string pass = "****";
        _logger.LogTrace("Connecting to smtp server...");
        
        var client = new SmtpClient("localhost", 1025);
        
    
        
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        client.Credentials = new System.Net.NetworkCredential(from, pass);
        client.EnableSsl = false;
        
        _logger.LogTrace("Connected to smtp server");
        var mail = new MailMessage(from, email);
        
        
        mail.Subject = subject;
        mail.Body = message;
        mail.IsBodyHtml = true;
        
        _logger.LogTrace("Sending email to {email}...", email);
        
        return client.SendMailAsync(mail);
    }
}