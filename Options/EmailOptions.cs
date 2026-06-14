namespace Needis.Web.Options;

public class EmailOptions
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = "Needis Website";
    public bool EnableSsl { get; set; } = true;
    public bool EnableEmailSending { get; set; } = false;
    public string AdminNotificationEmail { get; set; } = string.Empty;
}
