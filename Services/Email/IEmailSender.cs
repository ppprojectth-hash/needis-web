namespace Needis.Web.Services.Email;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody,
        CancellationToken cancellationToken = default);

    Task<bool> SendEmailAsync(List<string> toEmails, string subject, string htmlBody,
        CancellationToken cancellationToken = default);
}
