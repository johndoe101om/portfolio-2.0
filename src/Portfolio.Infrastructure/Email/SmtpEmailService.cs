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
        string? GetVal(string configKey, string envKey)
        {
            var v = config[configKey];
            if (!string.IsNullOrWhiteSpace(v)) return v;
            v = Environment.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrWhiteSpace(v)) return v;
            return config[envKey];
        }

        var smtpHost = GetVal("Email:SmtpHost", "SMTP_HOST");
        var portStr  = GetVal("Email:SmtpPort", "SMTP_PORT");
        var smtpPort = int.TryParse(portStr, out var configuredPort) ? configuredPort : 587;
        var smtpUser = GetVal("Email:SmtpUser", "SMTP_USER");
        var smtpPass = GetVal("Email:SmtpPass", "SMTP_PASS")?.Replace(" ", "").Trim();
        var toEmail  = GetVal("Email:ToAddress", "NOTIFY_EMAIL") ?? "sirsatyamchaudhary@gmail.com";

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
