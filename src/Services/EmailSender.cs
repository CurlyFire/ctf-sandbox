using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ctf_sandbox.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
        {
            Port = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "1025"),
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = false
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(
                _configuration["EmailSettings:FromEmail"] ?? "noreply@ctfarena.com",
                _configuration["EmailSettings:FromName"] ?? "CTF Arena"
            ),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}