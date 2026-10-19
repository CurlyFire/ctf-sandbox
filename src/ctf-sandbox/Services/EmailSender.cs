using System.Net.Mail;
using ctf_sandbox.Email;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ctf_sandbox.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    private readonly ISmtpClient _smtpClient;

    public EmailSender(IConfiguration configuration, ISmtpClient smtpClient)
    {
        _configuration = configuration;
        _smtpClient = smtpClient;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
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

        await _smtpClient.SendMailAsync(mailMessage);
    }
}