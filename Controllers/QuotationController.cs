using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.Options;
using Needis.Web.Services;
using Needis.Web.Services.Content;
using Needis.Web.Services.Email;
using Needis.Web.ViewModels.Quotation;

namespace Needis.Web.Controllers;

public class QuotationController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILanguageService _lang;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplate;
    private readonly EmailOptions _emailOpts;
    private readonly ILogger<QuotationController> _logger;
    private readonly ISiteTextService _siteText;

    private static readonly string[] TextKeys =
    [
        "quote.page.title", "quote.page.subtitle", "quote.submit_button",
    ];

    public QuotationController(
        AppDbContext db,
        ILanguageService lang,
        IEmailSender emailSender,
        IEmailTemplateService emailTemplate,
        IOptions<EmailOptions> emailOpts,
        ILogger<QuotationController> logger,
        ISiteTextService siteText)
    {
        _db            = db;
        _lang          = lang;
        _emailSender   = emailSender;
        _emailTemplate = emailTemplate;
        _emailOpts     = emailOpts.Value;
        _logger        = logger;
        _siteText      = siteText;
    }

    // ── GET /Quotation/Create ─────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Create(
        string? productSlug,
        int?    productId,
        string? serviceSlug,
        int?    serviceId)
    {
        var lang    = _lang.GetCurrentLanguage(HttpContext);
        var product = await LoadProductAsync(productSlug, productId);
        var service = await LoadServiceAsync(serviceSlug, serviceId);

        string subject;
        string requestType;

        if (product is not null)
        {
            requestType = "Product";
            subject = lang == "th"
                ? $"ขอใบเสนอราคาสำหรับ {product.NameTH}"
                : $"Quotation Request for {product.NameEN}";
        }
        else if (service is not null)
        {
            requestType = "Service";
            subject = lang == "th"
                ? $"ขอใบเสนอราคาสำหรับบริการ: {service.ServiceNameTH}"
                : $"Quotation request for service: {service.ServiceNameEN}";
        }
        else
        {
            requestType = "General";
            subject = lang == "th" ? "ขอใบเสนอราคาทั่วไป" : "General Quotation Request";
        }

        var vm = new QuotationRequestFormViewModel
        {
            CurrentLanguage = lang,
            Product         = product,
            ProductId       = product?.Id,
            ProductSlug     = product?.Slug,
            Service         = service,
            ServiceId       = service?.Id,
            ServiceSlug     = service?.ServiceSlug,
            Subject         = subject,
            RequestType     = requestType,
            Quantity        = 1,
            Texts           = await _siteText.GetTextsAsync(TextKeys, lang),
        };

        ViewData["FullWidth"]  = true;
        ViewData["SeoPageKey"] = "quotation";
        ViewData["SeoTitleEN"] = "Request Quotation";
        ViewData["SeoTitleTH"] = "ขอใบเสนอราคา";
        return View(vm);
    }

    // ── POST /Quotation/Create ────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuotationRequestFormViewModel model)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        model.CurrentLanguage = lang;

        // Always reload from DB — never trust snapshot from form POST
        var product = await LoadProductAsync(model.ProductSlug, model.ProductId);
        var service = await LoadServiceAsync(model.ServiceSlug, model.ServiceId);
        model.Product = product;
        model.Service = service;
        model.Texts   = await _siteText.GetTextsAsync(TextKeys, lang);

        ViewData["FullWidth"] = true;

        if (!ModelState.IsValid)
            return View(model);

        // Determine RequestType
        string requestType = (product, service) switch
        {
            (not null, null) => "Product",
            (null, not null) => "Service",
            (null, null)     => "General",
            _                => "Mixed",
        };

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var request = new QuotationRequest
        {
            RequestNo              = await GenerateRequestNoAsync(),
            CustomerName           = model.CustomerName,
            CompanyName            = model.CompanyName,
            Email                  = model.Email,
            Phone                  = model.Phone,
            PreferredContactMethod = model.PreferredContactMethod,
            Subject                = model.Subject,
            Message                = model.Message,
            RequestType            = requestType,
            Status                 = "New",
            Language               = lang,
            IpAddress              = ipAddress,
            UserAgent              = userAgent.Length > 500 ? userAgent[..500] : userAgent,
            CreatedAt              = DateTime.UtcNow,
        };

        _db.QuotationRequests.Add(request);
        await _db.SaveChangesAsync();

        var items = new List<QuotationRequestItem>();

        if (product is not null)
        {
            var item = new QuotationRequestItem
            {
                QuotationRequestId    = request.Id,
                ItemType              = "Product",
                ProductId             = product.Id,
                ProductNameSnapshotTH = product.NameTH,
                ProductNameSnapshotEN = product.NameEN,
                ProductSlugSnapshot   = product.Slug,
                BrandSnapshot         = product.Brand,
                ModelSnapshot         = product.Model,
                PartNumberSnapshot    = product.Sku,
                Quantity              = model.Quantity < 1 ? 1 : model.Quantity,
                ItemNote              = model.ItemNote,
                CreatedAt             = DateTime.UtcNow,
            };
            _db.QuotationRequestItems.Add(item);
            await _db.SaveChangesAsync();
            items.Add(item);
        }
        else if (service is not null)
        {
            var item = new QuotationRequestItem
            {
                QuotationRequestId   = request.Id,
                ItemType             = "Service",
                ServiceId            = service.Id,
                ServiceCodeSnapshot  = service.ServiceCode,
                ServiceNameSnapshotTH = service.ServiceNameTH,
                ServiceNameSnapshotEN = service.ServiceNameEN,
                ServiceSlugSnapshot  = service.ServiceSlug,
                Quantity             = model.Quantity < 1 ? 1 : model.Quantity,
                ItemNote             = model.ItemNote,
                CreatedAt            = DateTime.UtcNow,
            };
            _db.QuotationRequestItems.Add(item);
            await _db.SaveChangesAsync();
            items.Add(item);
        }

        await SendQuotationEmailsAsync(request, items, lang);

        TempData["QuoteSuccess"] = lang == "th"
            ? "ส่งคำขอใบเสนอราคาเรียบร้อยแล้ว"
            : "Your quotation request has been submitted successfully.";

        return RedirectToAction(nameof(ThankYou), new { requestNo = request.RequestNo });
    }

    // ── GET /Quotation/ThankYou ───────────────────────────────────────────────

    [HttpGet]
    public IActionResult ThankYou(string requestNo)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        ViewData["RequestNo"]       = requestNo;
        ViewData["CurrentLanguage"] = lang;
        ViewData["FullWidth"]       = true;
        return View();
    }

    // ── Email sending ────────────────────────────────────────────────────────

    private async Task SendQuotationEmailsAsync(
        QuotationRequest request,
        List<QuotationRequestItem> items,
        string lang)
    {
        if (!string.IsNullOrWhiteSpace(_emailOpts.AdminNotificationEmail))
        {
            var adminUrl     = $"{Request.Scheme}://{Request.Host}/Admin/QuotationRequest/Detail/{request.Id}";
            var adminSubject = $"New Quotation Request: {request.RequestNo}";
            var adminBody    = _emailTemplate.BuildQuotationAdminNotificationEmail(request, items, adminUrl);

            await SendAndLogAsync(_emailOpts.AdminNotificationEmail, adminSubject, adminBody,
                "QuotationAdminNotification", "QuotationRequest", request.Id);
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var customerSubject = lang == "th"
                ? $"ระบบได้รับคำขอใบเสนอราคาของคุณแล้ว {request.RequestNo}"
                : $"We received your quotation request {request.RequestNo}";
            var customerBody = _emailTemplate.BuildQuotationCustomerAutoReplyEmail(request, items, lang);

            await SendAndLogAsync(request.Email, customerSubject, customerBody,
                "QuotationCustomerAutoReply", "QuotationRequest", request.Id);
        }
    }

    private async Task SendAndLogAsync(
        string toEmail, string subject, string body,
        string emailType, string referenceType, int referenceId)
    {
        string status;
        string? error = null;

        try
        {
            var sent = await _emailSender.SendEmailAsync(toEmail, subject, body);
            status = !_emailOpts.EnableEmailSending ? "Skipped" : (sent ? "Success" : "Failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending {EmailType} to {Email}", emailType, toEmail);
            status = "Failed";
            error  = ex.Message;
        }

        try
        {
            _db.EmailSendLogs.Add(new EmailSendLog
            {
                EmailType     = emailType,
                ToEmail       = toEmail,
                Subject       = subject,
                ReferenceType = referenceType,
                ReferenceId   = referenceId,
                Status        = status,
                ErrorMessage  = error,
                CreatedAt     = DateTime.UtcNow,
            });
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save email log for {EmailType}", emailType);
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private async Task<Product?> LoadProductAsync(string? slug, int? id)
    {
        if (!string.IsNullOrEmpty(slug))
            return await _db.Products.AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

        if (id.HasValue)
            return await _db.Products.AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id.Value && p.IsActive);

        return null;
    }

    private async Task<Service?> LoadServiceAsync(string? slug, int? id)
    {
        if (!string.IsNullOrEmpty(slug))
            return await _db.Services.AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceSlug == slug && s.IsActive && !s.IsDelete);

        if (id.HasValue)
            return await _db.Services.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id.Value && s.IsActive && !s.IsDelete);

        return null;
    }

    private async Task<string> GenerateRequestNoAsync()
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd   = todayStart.AddDays(1);

        var count = await _db.QuotationRequests
            .CountAsync(r => r.CreatedAt >= todayStart && r.CreatedAt < todayEnd);

        return $"QR-{todayStart:yyyyMMdd}-{count + 1:D4}";
    }
}
