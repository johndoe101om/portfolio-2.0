using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Portfolio.Application.Interfaces;

namespace Portfolio.Infrastructure.Email;

/// <summary>
/// Sends emails via SMTP. Credentials are read from environment variables /
/// configuration. Never logged, never returned to the client.
/// </summary>
public class SmtpEmailService(IConfiguration config, ILogger<SmtpEmailService> logger) : IEmailService
{
    public async Task<bool> SendContactNotificationAsync(
        string name, string email, string subject, string message, CancellationToken ct = default)
    {
        var smtpHost = config["Email:SmtpHost"];
        var smtpPort = int.TryParse(config["Email:SmtpPort"], out var configuredPort)
            ? configuredPort
            : 587;
        var smtpUser = config["Email:SmtpUser"];
        var smtpPass = config["Email:SmtpPass"];  // from environment variable / secrets
        var toEmail  = config["Email:ToAddress"]  ?? "sirsatyamchaudhary@gmail.com";

        if (string.IsNullOrWhiteSpace(smtpHost) || string.IsNullOrWhiteSpace(smtpUser))
        {
            logger.LogWarning("SMTP not configured — contact message stored but not emailed.");
            return false;
        }

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
            };

            var mail = new MailMessage
            {
                From = new MailAddress(smtpUser, "Portfolio Contact Form"),
                Subject = $"[Portfolio] {subject}",
                Body = $"""
                    New contact message from your portfolio site.

                    From:    {name}
                    Email:   {email}
                    Subject: {subject}

                    Message:
                    {message}
                    """,
                IsBodyHtml = false,
            };

            mail.To.Add(toEmail);
            mail.ReplyToList.Add(new MailAddress(email, name));

            await client.SendMailAsync(mail, ct);
            return true;
        }
        catch (Exception ex)
        {
            // Log without exposing credentials
            logger.LogError("Failed to send contact email: {ErrorType}", ex.GetType().Name);
            return false;
        }
    }
}
