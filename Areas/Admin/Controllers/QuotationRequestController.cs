using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Quotation.View")]
public class QuotationRequestController : Controller
{
    private static readonly string[] ValidStatuses =
        ["New", "Contacted", "Quoted", "Closed", "Cancelled"];

    private static readonly string[] ValidRequestTypes =
        ["General", "Product", "Service", "Mixed"];

    private readonly AppDbContext _db;

    public QuotationRequestController(AppDbContext db) => _db = db;

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? dateFrom,
        DateTime? dateTo,
        string?   status,
        string?   requestType,
        string?   keyword)
    {
        ViewData["Title"] = "Quotation Requests";

        var now    = DateTime.UtcNow;
        dateFrom ??= now.AddDays(-30).Date;
        dateTo   ??= now.Date;

        var from = dateFrom.Value.Date;
        var to   = dateTo.Value.Date.AddDays(1);

        var query = _db.QuotationRequests
            .AsNoTracking()
            .Where(r => r.CreatedAt >= from && r.CreatedAt < to)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(r => r.Status == status);

        if (!string.IsNullOrWhiteSpace(requestType))
            query = query.Where(r => r.RequestType == requestType);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(r =>
                r.RequestNo.Contains(kw) ||
                (r.CustomerName != null && r.CustomerName.Contains(kw)) ||
                (r.CompanyName  != null && r.CompanyName.Contains(kw))  ||
                (r.Email        != null && r.Email.Contains(kw))        ||
                (r.Phone        != null && r.Phone.Contains(kw))        ||
                (r.Subject      != null && r.Subject.Contains(kw))      ||
                r.Items.Any(i =>
                    (i.ProductNameSnapshotEN  != null && i.ProductNameSnapshotEN.Contains(kw))  ||
                    (i.ProductNameSnapshotTH  != null && i.ProductNameSnapshotTH.Contains(kw))  ||
                    (i.ServiceNameSnapshotEN  != null && i.ServiceNameSnapshotEN.Contains(kw))  ||
                    (i.ServiceNameSnapshotTH  != null && i.ServiceNameSnapshotTH.Contains(kw))  ||
                    (i.ServiceCodeSnapshot    != null && i.ServiceCodeSnapshot.Contains(kw))));
        }

        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new QuotationRequestListItemViewModel
            {
                Id           = r.Id,
                RequestNo    = r.RequestNo,
                CustomerName = r.CustomerName,
                CompanyName  = r.CompanyName,
                Email        = r.Email,
                Phone        = r.Phone,
                Subject      = r.Subject,
                Status       = r.Status,
                RequestType  = r.RequestType,
                ItemCount    = r.Items.Count(),
                ItemSummary  = r.Items
                    .Select(i => i.ItemType == "Service"
                        ? (i.ServiceNameSnapshotEN ?? i.ServiceCodeSnapshot ?? "Service")
                        : (i.ProductNameSnapshotEN ?? "Product"))
                    .FirstOrDefault(),
                CreatedAt    = r.CreatedAt,
            })
            .ToListAsync();

        ViewBag.DateFrom          = dateFrom.Value.ToString("yyyy-MM-dd");
        ViewBag.DateTo            = dateTo.Value.ToString("yyyy-MM-dd");
        ViewBag.Status            = status;
        ViewBag.RequestType       = requestType;
        ViewBag.Keyword           = keyword;
        ViewBag.ValidStatuses     = ValidStatuses;
        ViewBag.ValidRequestTypes = ValidRequestTypes;

        return View(items);
    }

    // ── Detail ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        ViewData["Title"] = "Quotation Request Detail";

        var request = await _db.QuotationRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request is null) return NotFound();

        var items = await _db.QuotationRequestItems
            .AsNoTracking()
            .Include(i => i.Product)
            .Include(i => i.ServiceItem)
            .Where(i => i.QuotationRequestId == id)
            .ToListAsync();

        var vm = new QuotationRequestDetailViewModel
        {
            Request     = request,
            Items       = items,
            NewStatus   = request.Status,
            AdminRemark = request.AdminRemark,
        };

        return View(vm);
    }

    // ── UpdateStatus ─────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, string? adminRemark)
    {
        if (!ValidStatuses.Contains(status))
        {
            TempData["Error"] = "Invalid status value.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        var request = await _db.QuotationRequests.FindAsync(id);
        if (request is null) return NotFound();

        request.Status      = status;
        request.AdminRemark = adminRemark;
        request.UpdatedAt   = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Status updated successfully.";
        return RedirectToAction(nameof(Detail), new { id });
    }
}
