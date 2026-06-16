using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ContactMessageController : Controller
{
    private readonly AppDbContext _db;

    private static readonly string[] ValidStatuses = ["New", "Read", "Replied", "Closed", "Spam"];

    public ContactMessageController(AppDbContext db) => _db = db;

    // ── GET Index ─────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("ContactMessage.View")]
    public async Task<IActionResult> Index(
        DateTime? dateFrom, DateTime? dateTo,
        string? status, string? keyword, string? assignedTo)
    {
        var nowBangkok = BangkokTimeHelper.NowBangkok();
        var startDate  = dateFrom ?? nowBangkok.Date.AddDays(-30);
        var endDate    = dateTo   ?? nowBangkok.Date;

        var from         = BangkokTimeHelper.ConvertBangkokDateStartToUtc(startDate);
        var toExclusive  = BangkokTimeHelper.ConvertBangkokDateEndExclusiveToUtc(endDate);

        var query = _db.ContactMessages.AsNoTracking()
            .Where(c => c.CreatedAt >= from && c.CreatedAt < toExclusive);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(assignedTo))
            query = query.Where(c => c.AssignedTo == assignedTo);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(c =>
                c.FullName.Contains(kw) ||
                c.Email.Contains(kw)    ||
                c.Subject.Contains(kw)  ||
                c.Message.Contains(kw)  ||
                (c.Phone       != null && c.Phone.Contains(kw))       ||
                (c.AdminRemark != null && c.AdminRemark.Contains(kw)));
        }

        var messages = await query
            .OrderByDescending(c => c.CreatedAt)
            .Take(300)
            .Select(c => new ContactMessageListItemViewModel
            {
                Id         = c.Id,
                FullName   = c.FullName,
                Email      = c.Email,
                Phone      = c.Phone,
                Subject    = c.Subject,
                Status     = c.Status ?? "New",
                AssignedTo = c.AssignedTo,
                CreatedAt  = c.CreatedAt,
                ReadAt     = c.ReadAt,
                RepliedAt  = c.RepliedAt,
                ClosedAt   = c.ClosedAt,
            })
            .ToListAsync();

        // Summary counts across all time for selected status range
        var countQuery = _db.ContactMessages.AsNoTracking()
            .Where(c => c.CreatedAt >= from && c.CreatedAt < toExclusive);

        var totalCount   = await countQuery.CountAsync();
        var newCount     = await countQuery.CountAsync(c => c.Status == "New"     || c.Status == null);
        var readCount    = await countQuery.CountAsync(c => c.Status == "Read");
        var repliedCount = await countQuery.CountAsync(c => c.Status == "Replied");
        var closedCount  = await countQuery.CountAsync(c => c.Status == "Closed");
        var spamCount    = await countQuery.CountAsync(c => c.Status == "Spam");

        var vm = new ContactMessageIndexViewModel
        {
            DateFrom     = startDate.Date,
            DateTo       = endDate.Date,
            Status       = status,
            Keyword      = keyword,
            AssignedTo   = assignedTo,
            Messages     = messages,
            TotalCount   = totalCount,
            NewCount     = newCount,
            ReadCount    = readCount,
            RepliedCount = repliedCount,
            ClosedCount  = closedCount,
            SpamCount    = spamCount,
        };

        return View(vm);
    }

    // ── GET Detail ───────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("ContactMessage.View")]
    public async Task<IActionResult> Detail(int id)
    {
        var message = await _db.ContactMessages.FirstOrDefaultAsync(c => c.Id == id);
        if (message is null) return NotFound();

        // Auto-mark as Read on first open
        if (message.Status is null or "New")
        {
            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;
            message.Status    = "Read";
            message.IsRead    = true;
            message.ReadAt    = DateTime.UtcNow;
            message.UpdatedAt = DateTime.UtcNow;
            message.UpdatedBy = username;
            await _db.SaveChangesAsync();
        }

        var vm = new ContactMessageDetailViewModel
        {
            Message     = message,
            NewStatus   = message.Status,
            AdminRemark = message.AdminRemark,
            AssignedTo  = message.AssignedTo,
        };

        return View(vm);
    }

    // ── POST UpdateStatus ─────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ContactMessage.Edit")]
    public async Task<IActionResult> UpdateStatus(ContactMessageUpdateViewModel model)
    {
        if (!ValidStatuses.Contains(model.Status))
            ModelState.AddModelError(nameof(model.Status), "Invalid status value.");

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Invalid status selected.";
            return RedirectToAction(nameof(Detail), new { id = model.Id });
        }

        var message = await _db.ContactMessages.FindAsync(model.Id);
        if (message is null) return NotFound();

        var username = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;

        message.Status      = model.Status;
        message.AdminRemark = model.AdminRemark;
        message.AssignedTo  = model.AssignedTo;
        message.UpdatedAt   = DateTime.UtcNow;
        message.UpdatedBy   = username;

        if (model.Status == "Read"    && message.ReadAt    is null) message.ReadAt    = DateTime.UtcNow;
        if (model.Status == "Replied" && message.RepliedAt is null) message.RepliedAt = DateTime.UtcNow;
        if (model.Status == "Closed"  && message.ClosedAt  is null) message.ClosedAt  = DateTime.UtcNow;

        if (model.Status is "Read" or "Replied" or "Closed" or "Spam")
            message.IsRead = true;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Message status updated to '{model.Status}'.";
        return RedirectToAction(nameof(Detail), new { id = model.Id });
    }

    // ── POST MarkAsSpam ───────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ContactMessage.Edit")]
    public async Task<IActionResult> MarkAsSpam(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();

        message.Status    = "Spam";
        message.IsRead    = true;
        message.UpdatedAt = DateTime.UtcNow;
        message.UpdatedBy = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Message marked as spam.";
        return RedirectToAction(nameof(Detail), new { id });
    }

    // ── POST Close ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ContactMessage.Edit")]
    public async Task<IActionResult> Close(int id)
    {
        var message = await _db.ContactMessages.FindAsync(id);
        if (message is null) return NotFound();

        message.Status    = "Closed";
        message.IsRead    = true;
        message.ClosedAt  = message.ClosedAt ?? DateTime.UtcNow;
        message.UpdatedAt = DateTime.UtcNow;
        message.UpdatedBy = User.FindFirstValue(ClaimTypes.Name) ?? User.Identity?.Name;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Message closed.";
        return RedirectToAction(nameof(Detail), new { id });
    }

    // ── GET ExportCsv ─────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("ContactMessage.View")]
    public async Task<IActionResult> ExportCsv(
        DateTime? dateFrom, DateTime? dateTo,
        string? status, string? keyword)
    {
        var nowBangkok = BangkokTimeHelper.NowBangkok();
        var startDate  = dateFrom ?? nowBangkok.Date.AddDays(-30);
        var endDate    = dateTo   ?? nowBangkok.Date;

        var from        = BangkokTimeHelper.ConvertBangkokDateStartToUtc(startDate);
        var toExclusive = BangkokTimeHelper.ConvertBangkokDateEndExclusiveToUtc(endDate);

        var query = _db.ContactMessages.AsNoTracking()
            .Where(c => c.CreatedAt >= from && c.CreatedAt < toExclusive);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(c => c.Status == status);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(c =>
                c.FullName.Contains(kw) ||
                c.Email.Contains(kw)    ||
                c.Subject.Contains(kw)  ||
                c.Message.Contains(kw));
        }

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Take(5000)
            .ToListAsync();

        var csv = new StringBuilder();
        csv.AppendLine("CreatedAt,FullName,Email,Phone,Subject,Message,Status,AssignedTo,AdminRemark");

        foreach (var c in items)
        {
            csv.AppendLine(string.Join(',', [
                CsvEscape(c.CreatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm")),
                CsvEscape(c.FullName),
                CsvEscape(c.Email),
                CsvEscape(c.Phone),
                CsvEscape(c.Subject),
                CsvEscape(c.Message),
                CsvEscape(c.Status ?? "New"),
                CsvEscape(c.AssignedTo),
                CsvEscape(c.AdminRemark),
            ]));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
        return File(bytes, "text/csv", $"contact-messages-{DateTime.Now:yyyyMMdd}.csv");
    }

    private static string CsvEscape(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Contains(',') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
    }
}
