using GearFlow.Domain.Interfaces;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace GearFlow.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_configuration["Email:From"]));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        await smtp.ConnectAsync(
            _configuration["Email:Host"],
            int.Parse(_configuration["Email:Port"]!),
            SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(
            _configuration["Email:Username"],
            _configuration["Email:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}