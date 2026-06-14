using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [RequirePermission("Dashboard.View")]
    public async Task<IActionResult> Index()
    {
        var today        = DateTime.UtcNow.Date;
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

        // ── Products (no IsDelete field) ──────────────────────────────────
        var totalProducts  = await _db.Products.AsNoTracking().CountAsync();
        var activeProducts = await _db.Products.AsNoTracking().CountAsync(p => p.IsActive);

        // ── Categories (no IsDelete field) ───────────────────────────────
        var totalCategories = await _db.ProductCategories.AsNoTracking().CountAsync();

        // ── Services ─────────────────────────────────────────────────────
        var totalServices  = await _db.Services.AsNoTracking().CountAsync(s => !s.IsDelete);
        var activeServices = await _db.Services.AsNoTracking().CountAsync(s => s.IsActive && !s.IsDelete);

        // ── Activities ───────────────────────────────────────────────────
        var totalActivities     = await _db.Activities.AsNoTracking().CountAsync(a => !a.IsDelete);
        var publishedActivities = await _db.Activities.AsNoTracking()
            .CountAsync(a => a.IsPublished && a.IsActive && !a.IsDelete);

        // ── Quotation Requests ────────────────────────────────────────────
        var newQuotationRequests   = await _db.QuotationRequests.AsNoTracking().CountAsync(q => q.Status == "New");
        var totalQuotationRequests = await _db.QuotationRequests.AsNoTracking().CountAsync();
        var latestQuotations = await _db.QuotationRequests.AsNoTracking()
            .OrderByDescending(q => q.CreatedAt)
            .Take(5)
            .Select(q => new DashboardQuotationItemViewModel
            {
                Id           = q.Id,
                RequestNo    = q.RequestNo,
                CustomerName = q.CustomerName,
                CompanyName  = q.CompanyName,
                Email        = q.Email,
                Subject      = q.Subject,
                RequestType  = q.RequestType,
                Status       = q.Status,
                CreatedAt    = q.CreatedAt,
            })
            .ToListAsync();

        // ── Contact Messages ──────────────────────────────────────────────
        var newContactMessages   = await _db.ContactMessages.AsNoTracking().CountAsync(c => c.Status == "New" || c.Status == null);
        var totalContactMessages = await _db.ContactMessages.AsNoTracking().CountAsync();
        var latestContacts = await _db.ContactMessages.AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => new DashboardContactMessageItemViewModel
            {
                Id        = c.Id,
                FullName  = c.FullName,
                Email     = c.Email,
                Subject   = c.Subject,
                CreatedAt = c.CreatedAt,
            })
            .ToListAsync();

        // ── Usage Logs ────────────────────────────────────────────────────
        var todayUsageCount  = await _db.UsageLogs.AsNoTracking()
            .CountAsync(u => u.AccessedAt >= today);
        var failedUsageCount = await _db.UsageLogs.AsNoTracking()
            .CountAsync(u => !u.IsSuccess && u.AccessedAt >= sevenDaysAgo);

        var topPages = await _db.UsageLogs.AsNoTracking()
            .Where(u => u.AccessedAt >= sevenDaysAgo)
            .GroupBy(u => new { u.PageName, u.PageUrl })
            .Select(g => new DashboardTopPageItemViewModel
            {
                PageName = g.Key.PageName,
                Url      = g.Key.PageUrl,
                Count    = g.Count(),
            })
            .OrderByDescending(p => p.Count)
            .Take(10)
            .ToListAsync();

        var dailyUsage = await _db.UsageLogs.AsNoTracking()
            .Where(u => u.AccessedAt >= sevenDaysAgo)
            .GroupBy(u => u.AccessedAt.Date)
            .Select(g => new DashboardDailyUsageItemViewModel
            {
                Date  = g.Key,
                Count = g.Count(),
            })
            .OrderBy(d => d.Date)
            .ToListAsync();

        // ── Email Send Logs (last 7 days) ─────────────────────────────────
        var emailSuccessCount = await _db.EmailSendLogs.AsNoTracking()
            .CountAsync(e => e.Status == "Success" && e.CreatedAt >= sevenDaysAgo);
        var emailFailedCount = await _db.EmailSendLogs.AsNoTracking()
            .CountAsync(e => e.Status == "Failed" && e.CreatedAt >= sevenDaysAgo);
        var emailSkippedCount = await _db.EmailSendLogs.AsNoTracking()
            .CountAsync(e => e.Status == "Skipped" && e.CreatedAt >= sevenDaysAgo);

        // ── Top Products (from usage logs, last 7 days) ───────────────────
        var productDetailLogs = await _db.UsageLogs.AsNoTracking()
            .Where(u => u.AccessedAt >= sevenDaysAgo
                     && u.PageUrl != null
                     && u.PageUrl.Contains("/Product/Detail"))
            .GroupBy(u => new { u.PageUrl, u.PageName })
            .Select(g => new { Url = g.Key.PageUrl, Name = g.Key.PageName, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToListAsync();

        // Extract slugs and resolve product names in a single lookup
        var slugsWithMeta = productDetailLogs
            .Select(x =>
            {
                var url = x.Url ?? string.Empty;
                var idx = url.LastIndexOf('/');
                return new { Slug = idx >= 0 ? url[(idx + 1)..] : url, x.Name, x.Count };
            })
            .ToList();

        var slugList = slugsWithMeta.Select(s => s.Slug).ToList();
        var productLookup = await _db.Products.AsNoTracking()
            .Where(p => slugList.Contains(p.Slug))
            .Select(p => new { p.Id, p.NameEN, p.NameTH, p.Slug })
            .ToListAsync();
        var productDict = productLookup.ToDictionary(p => p.Slug);

        var topProducts = slugsWithMeta.Select(s =>
        {
            productDict.TryGetValue(s.Slug, out var product);
            return new DashboardTopProductItemViewModel
            {
                ProductId     = product?.Id ?? 0,
                ProductNameEN = product?.NameEN ?? s.Name ?? s.Slug,
                ProductNameTH = product?.NameTH,
                Slug          = s.Slug,
                ViewCount     = s.Count,
            };
        }).ToList();

        var vm = new AdminDashboardViewModel
        {
            TotalProducts           = totalProducts,
            ActiveProducts          = activeProducts,
            TotalCategories         = totalCategories,
            TotalServices           = totalServices,
            ActiveServices          = activeServices,
            TotalActivities         = totalActivities,
            PublishedActivities     = publishedActivities,
            NewQuotationRequests    = newQuotationRequests,
            TotalQuotationRequests  = totalQuotationRequests,
            NewContactMessages      = newContactMessages,
            TotalContactMessages    = totalContactMessages,
            TodayUsageCount         = todayUsageCount,
            FailedUsageCount        = failedUsageCount,
            EmailSuccessCount       = emailSuccessCount,
            EmailFailedCount        = emailFailedCount,
            EmailSkippedCount       = emailSkippedCount,
            LatestQuotationRequests = latestQuotations,
            LatestContactMessages   = latestContacts,
            TopPages                = topPages,
            TopProducts             = topProducts,
            DailyUsage              = dailyUsage,
        };

        return View(vm);
    }
}
