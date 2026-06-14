using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Needis.Web.Options;

namespace Needis.Web.Services.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly EmailOptions _opts;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IOptions<EmailOptions> opts, ILogger<SmtpEmailSender> logger)
    {
        _opts   = opts.Value;
        _logger = logger;
    }

    public Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
        => SendEmailAsync([toEmail], subject, htmlBody, cancellationToken);

    public async Task<bool> SendEmailAsync(List<string> toEmails, string subject, string htmlBody,
        CancellationToken cancellationToken = default)
    {
        if (!_opts.EnableEmailSending)
        {
            _logger.LogInformation(
                "Email sending disabled. Would send to {Recipients} with subject: {Subject}",
                string.Join(", ", toEmails), subject);
            return true;
        }

        try
        {
#pragma warning disable SYSLIB0006 // SmtpClient is still functional; MailKit may be adopted later
            using var client = new SmtpClient(_opts.SmtpHost, _opts.SmtpPort)
            {
                EnableSsl   = _opts.EnableSsl,
                Credentials = new NetworkCredential(_opts.SmtpUsername, _opts.SmtpPassword),
            };
#pragma warning restore SYSLIB0006

            using var message = new MailMessage
            {
                From       = new MailAddress(_opts.FromEmail, _opts.FromName),
                Subject    = subject,
                Body       = htmlBody,
                IsBodyHtml = true,
            };

            foreach (var to in toEmails)
                message.To.Add(to);

            await client.SendMailAsync(message, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully to {Recipients} with subject: {Subject}",
                string.Join(", ", toEmails), subject);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send email to {Recipients} with subject: {Subject}",
                string.Join(", ", toEmails), subject);
            return false;
        }
    }
}
