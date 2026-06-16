using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;

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

        var nowBangkok = BangkokTimeHelper.NowBangkok();
        dateFrom ??= nowBangkok.Date.AddDays(-7);
        dateTo   ??= nowBangkok.Date;

        var from        = BangkokTimeHelper.ConvertBangkokDateStartToUtc(dateFrom.Value);
        var toExclusive = BangkokTimeHelper.ConvertBangkokDateEndExclusiveToUtc(dateTo.Value);

        var query = _db.EmailSendLogs
            .AsNoTracking()
            .Where(l => l.CreatedAt >= from && l.CreatedAt < toExclusive)
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
