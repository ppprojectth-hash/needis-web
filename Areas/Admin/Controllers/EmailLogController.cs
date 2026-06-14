using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("EmailLog.View")]
public class EmailLogController : Controller
{
    private readonly AppDbContext _db;

    public EmailLogController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(
        DateTime? dateFrom,
        DateTime? dateTo,
        string?   status,
        string?   keyword)
    {
        ViewData["Title"] = "Email Logs";

        var now    = DateTime.UtcNow;
        dateFrom ??= now.AddDays(-7).Date;
        dateTo   ??= now.Date;

        var from = dateFrom.Value.Date;
        var to   = dateTo.Value.Date.AddDays(1);

        var query = _db.EmailSendLogs
            .AsNoTracking()
            .Where(l => l.CreatedAt >= from && l.CreatedAt < to)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(l => l.Status == status);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(l =>
                (l.ToEmail       != null && l.ToEmail.Contains(kw))       ||
                (l.Subject       != null && l.Subject.Contains(kw))       ||
                (l.EmailType     != null && l.EmailType.Contains(kw))     ||
                (l.ReferenceType != null && l.ReferenceType.Contains(kw)) ||
                (l.ErrorMessage  != null && l.ErrorMessage.Contains(kw)));
        }

        var logs = await query
            .OrderByDescending(l => l.CreatedAt)
            .Take(200)
            .ToListAsync();

        ViewBag.DateFrom = dateFrom.Value.ToString("yyyy-MM-dd");
        ViewBag.DateTo   = dateTo.Value.ToString("yyyy-MM-dd");
        ViewBag.Status   = status;
        ViewBag.Keyword  = keyword;

        return View(logs);
    }
}
