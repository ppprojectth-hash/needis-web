using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.Options;
using Needis.Web.Services;
using Needis.Web.Services.Email;
using Needis.Web.ViewModels.Contact;

namespace Needis.Web.Controllers;

public class ContactController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILanguageService _lang;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplate;
    private readonly EmailOptions _emailOpts;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
        AppDbContext db,
        ILanguageService lang,
        IEmailSender emailSender,
        IEmailTemplateService emailTemplate,
        IOptions<EmailOptions> emailOpts,
        ILogger<ContactController> logger)
    {
        _db            = db;
        _lang          = lang;
        _emailSender   = emailSender;
        _emailTemplate = emailTemplate;
        _emailOpts     = emailOpts.Value;
        _logger        = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        var vm = new ContactPageViewModel
        {
            CurrentLanguage = lang,
            SiteSetting     = await LoadSiteSettingAsync(),
            FooterContact   = await LoadFooterContactAsync(),
            Form            = new ContactFormViewModel(),
        };

        ViewData["SeoPageKey"] = "contact";
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ContactFormViewModel Form)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        if (!ModelState.IsValid)
        {
            var vm = new ContactPageViewModel
            {
                CurrentLanguage = lang,
                SiteSetting     = await LoadSiteSettingAsync(),
                FooterContact   = await LoadFooterContactAsync(),
                Form            = Form,
            };
            return View(vm);
        }

        var message = new ContactMessage
        {
            FullName  = Form.FullName  ?? string.Empty,
            Email     = Form.Email     ?? string.Empty,
            Phone     = Form.Phone,
            Subject   = Form.Subject   ?? string.Empty,
            Message   = Form.Message   ?? string.Empty,
            Status    = "New",
            IsRead    = false,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = HttpContext.Request.Headers.UserAgent.FirstOrDefault(),
            Language  = lang,
            CreatedAt = DateTime.UtcNow,
        };

        _db.ContactMessages.Add(message);
        await _db.SaveChangesAsync();

        // Notify admin — failure must not break the contact form
        var adminUrl = $"{Request.Scheme}://{Request.Host}/Admin/ContactMessage/Detail/{message.Id}";
        await SendContactNotificationAsync(message, adminUrl);

        TempData["ContactSuccess"] = lang == "th"
            ? "ส่งข้อความเรียบร้อยแล้ว"
            : "Your message has been sent successfully.";

        return RedirectToAction(nameof(Index));
    }

    // ── Email notification ───────────────────────────────────────────────────

    private async Task SendContactNotificationAsync(ContactMessage message, string? adminDetailUrl = null)
    {
        if (string.IsNullOrWhiteSpace(_emailOpts.AdminNotificationEmail))
            return;

        var subject = $"New Contact Message: {message.Subject} from {message.FullName}";
        var body    = _emailTemplate.BuildContactMessageNotificationEmail(message, adminDetailUrl);
        string status;
        string? error = null;

        try
        {
            var sent = await _emailSender.SendEmailAsync(_emailOpts.AdminNotificationEmail, subject, body);
            status = !_emailOpts.EnableEmailSending ? "Skipped" : (sent ? "Success" : "Failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send contact notification email");
            status = "Failed";
            error  = ex.Message;
        }

        try
        {
            _db.EmailSendLogs.Add(new EmailSendLog
            {
                EmailType     = "ContactMessageNotification",
                ToEmail       = _emailOpts.AdminNotificationEmail,
                Subject       = subject,
                ReferenceType = "ContactMessage",
                ReferenceId   = message.Id,
                Status        = status,
                ErrorMessage  = error,
                CreatedAt     = DateTime.UtcNow,
            });
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save email log for contact notification");
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private Task<SiteSetting?> LoadSiteSettingAsync() =>
        _db.SiteSettings.AsNoTracking().Where(s => s.IsActive).FirstOrDefaultAsync();

    private Task<FooterContact?> LoadFooterContactAsync() =>
        _db.FooterContacts.AsNoTracking().Where(f => f.IsActive).FirstOrDefaultAsync();
}
