using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

public class GmailEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public GmailEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var senderEmail = _configuration["Gmail:SmtpEmail"];
        var senderPassword = _configuration["Gmail:SmtpPassword"];

        if (string.IsNullOrEmpty(senderEmail))
            throw new InvalidOperationException("Gmail sender email is not configured.");
        if (string.IsNullOrEmpty(senderPassword))
            throw new InvalidOperationException("Gmail sender password is not configured.");

        var client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(senderEmail, senderPassword),
            EnableSsl = true,
        };

        var message = new MailMessage(senderEmail!, email, subject ?? "", htmlMessage ?? "")
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(message);
    }
}
