using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.Options;
using Needis.Web.Services;
using Needis.Web.Services.Content;
using Needis.Web.Services.Email;
using Needis.Web.Services.Quotation;
using Needis.Web.ViewModels.Quotation;

namespace Needis.Web.Controllers;

public class QuotationCartController : Controller
{
    private readonly AppDbContext _db;
    private readonly ILanguageService _lang;
    private readonly IQuotationCartService _cartService;
    private readonly IEmailSender _emailSender;
    private readonly IEmailTemplateService _emailTemplate;
    private readonly EmailOptions _emailOpts;
    private readonly ILogger<QuotationCartController> _logger;
    private readonly ISiteTextService _siteText;

    public QuotationCartController(
        AppDbContext db,
        ILanguageService lang,
        IQuotationCartService cartService,
        IEmailSender emailSender,
        IEmailTemplateService emailTemplate,
        IOptions<EmailOptions> emailOpts,
        ILogger<QuotationCartController> logger,
        ISiteTextService siteText)
    {
        _db            = db;
        _lang          = lang;
        _cartService   = cartService;
        _emailSender   = emailSender;
        _emailTemplate = emailTemplate;
        _emailOpts     = emailOpts.Value;
        _logger        = logger;
        _siteText      = siteText;
    }

    // ── GET /QuotationCart ───────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);
        var cart = await _cartService.GetActiveCartWithItemsAsync(HttpContext);

        var vm = new QuotationCartViewModel
        {
            CurrentLanguage = lang,
            Items           = cart?.Items.ToList() ?? new List<QuotationCartItem>(),
            Texts           = await _siteText.GetTextsAsync(["quote.empty_cart"], lang),
        };

        if (vm.TotalItems > 0)
        {
            vm.Subject = lang == "th"
                ? "ขอใบเสนอราคาสินค้า/บริการ"
                : "Quotation Request";
        }

        ViewData["SeoPageKey"] = "quotation";
        ViewData["SeoTitleTH"] = "รายการขอใบเสนอราคา";
        ViewData["SeoTitleEN"] = "Quotation Cart";
        ViewData["FullWidth"]  = true;
        return View(vm);
    }

    // ── POST /QuotationCart/AddProduct ───────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(
        int productId,
        int quantity = 1,
        string? itemNote = null,
        string? returnUrl = null)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        await _cartService.AddProductAsync(HttpContext, productId, quantity, itemNote);

        TempData["CartSuccess"] = lang == "th"
            ? "เพิ่มสินค้าในรายการขอใบเสนอราคาแล้ว"
            : "Product added to quotation cart.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // ── POST /QuotationCart/AddService ───────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddService(
        int serviceId,
        int quantity = 1,
        string? itemNote = null,
        string? returnUrl = null)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        await _cartService.AddServiceAsync(HttpContext, serviceId, quantity, itemNote);

        TempData["CartSuccess"] = lang == "th"
            ? "เพิ่มบริการในรายการขอใบเสนอราคาแล้ว"
            : "Service added to quotation cart.";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction(nameof(Index));
    }

    // ── POST /QuotationCart/UpdateQuantity ───────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateQuantity(int itemId, int quantity)
    {
        await _cartService.UpdateQuantityAsync(HttpContext, itemId, quantity);
        return RedirectToAction(nameof(Index));
    }

    // ── POST /QuotationCart/UpdateNote ───────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNote(int itemId, string? itemNote)
    {
        await _cartService.UpdateNoteAsync(HttpContext, itemId, itemNote);
        return RedirectToAction(nameof(Index));
    }

    // ── POST /QuotationCart/RemoveItem ───────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        await _cartService.RemoveItemAsync(HttpContext, itemId);
        return RedirectToAction(nameof(Index));
    }

    // ── POST /QuotationCart/Clear ────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearCartAsync(HttpContext);
        return RedirectToAction(nameof(Index));
    }

    // ── POST /QuotationCart/Submit ───────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(QuotationCartSubmitViewModel model)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        var cart = await _cartService.GetActiveCartWithItemsAsync(HttpContext);

        if (cart is null || !cart.Items.Any())
        {
            TempData["CartError"] = lang == "th"
                ? "ไม่มีรายการในรายการขอใบเสนอราคา"
                : "Your quotation cart is empty.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            var vm = new QuotationCartViewModel
            {
                CurrentLanguage = lang,
                Items           = cart.Items.ToList(),
                CustomerName    = model.CustomerName,
                CompanyName     = model.CompanyName,
                Email           = model.Email,
                Phone           = model.Phone,
                PreferredContactMethod = model.PreferredContactMethod,
                Subject         = model.Subject,
                Message         = model.Message,
            };
            ViewData["SeoPageKey"] = "quotation";
            ViewData["FullWidth"]  = true;
            return View("Index", vm);
        }

        var now       = DateTime.UtcNow;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        // Determine request type from cart items
        var hasProducts = cart.Items.Any(i => i.ItemType == "Product");
        var hasServices = cart.Items.Any(i => i.ItemType == "Service");
        var requestType = (hasProducts, hasServices) switch
        {
            (true, true)   => "Mixed",
            (true, false)  => "Product",
            (false, true)  => "Service",
            _              => "General",
        };

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
            CreatedAt              = now,
        };

        _db.QuotationRequests.Add(request);
        await _db.SaveChangesAsync();

        // Build QuotationRequestItems from cart items (use snapshots — safe even if product/service deleted)
        var requestItems = new List<QuotationRequestItem>();
        foreach (var cartItem in cart.Items)
        {
            var ri = new QuotationRequestItem
            {
                QuotationRequestId    = request.Id,
                ItemType              = cartItem.ItemType,
                ProductId             = cartItem.ProductId,
                ProductNameSnapshotTH = cartItem.ProductNameSnapshotTH,
                ProductNameSnapshotEN = cartItem.ProductNameSnapshotEN,
                ProductSlugSnapshot   = cartItem.ProductSlugSnapshot,
                BrandSnapshot         = cartItem.BrandSnapshot,
                ModelSnapshot         = cartItem.ModelSnapshot,
                PartNumberSnapshot    = cartItem.PartNumberSnapshot,
                ServiceId             = cartItem.ServiceId,
                ServiceCodeSnapshot   = cartItem.ServiceCodeSnapshot,
                ServiceNameSnapshotTH = cartItem.ServiceNameSnapshotTH,
                ServiceNameSnapshotEN = cartItem.ServiceNameSnapshotEN,
                ServiceSlugSnapshot   = cartItem.ServiceSlugSnapshot,
                Quantity              = cartItem.Quantity,
                ItemNote              = cartItem.ItemNote,
                CreatedAt             = now,
            };
            requestItems.Add(ri);
        }

        _db.QuotationRequestItems.AddRange(requestItems);

        // Mark cart submitted
        cart.IsSubmitted  = true;
        cart.SubmittedAt  = now;
        cart.UpdatedAt    = now;

        await _db.SaveChangesAsync();

        // Send email notifications (failure must not break the flow)
        await SendQuotationEmailsAsync(request, requestItems, lang);

        return RedirectToAction("ThankYou", "Quotation", new { requestNo = request.RequestNo });
    }

    // ── Email helpers ────────────────────────────────────────────────────────

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
            _logger.LogError(ex, "Error sending {EmailType}", emailType);
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

    private async Task<string> GenerateRequestNoAsync()
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd   = todayStart.AddDays(1);
        var count = await _db.QuotationRequests
            .CountAsync(r => r.CreatedAt >= todayStart && r.CreatedAt < todayEnd);
        return $"QR-{todayStart:yyyyMMdd}-{count + 1:D4}";
    }
}
