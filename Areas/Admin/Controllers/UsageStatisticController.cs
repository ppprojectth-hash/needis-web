using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("UsageStatistic.View")]
public class UsageStatisticController : Controller
{
    private readonly AppDbContext _db;

    public UsageStatisticController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo, string? keyword)
    {
        var todayBangkok = BangkokTimeHelper.NowBangkok().Date;
        var startDate    = dateFrom?.Date ?? todayBangkok.AddDays(-7);
        var endDate      = dateTo?.Date   ?? todayBangkok;

        var from = BangkokTimeHelper.ConvertBangkokDateStartToUtc(startDate);
        var to   = BangkokTimeHelper.ConvertBangkokDateEndExclusiveToUtc(endDate);

        var query = _db.UsageLogs
            .AsNoTracking()
            .Where(x => x.AccessedAt >= from && x.AccessedAt < to);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim().ToLower();
            query = query.Where(x =>
                (x.PageName     != null && x.PageName.ToLower().Contains(kw))     ||
                (x.Path         != null && x.Path.ToLower().Contains(kw))         ||
                (x.Username     != null && x.Username.ToLower().Contains(kw))     ||
                (x.FunctionName != null && x.FunctionName.ToLower().Contains(kw)));
        }

        var logs = await query
            .OrderByDescending(x => x.AccessedAt)
            .ToListAsync();

        var total   = logs.Count;
        var success = logs.Count(x => x.IsSuccess);
        var avgMs   = total > 0 ? logs.Average(x => x.DurationMs) : 0.0;

        var daily = logs
            .GroupBy(x => DateOnly.FromDateTime(x.AccessedAt))
            .OrderByDescending(g => g.Key)
            .Select(g => new DailyUsageItemViewModel
            {
                Date              = g.Key,
                TotalRequests     = g.Count(),
                SuccessRequests   = g.Count(x => x.IsSuccess),
                FailedRequests    = g.Count(x => !x.IsSuccess),
                AverageDurationMs = g.Average(x => x.DurationMs),
            })
            .ToList();

        var topPages = logs
            .Where(x => x.PageName is not null)
            .GroupBy(x => x.PageName!)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => new TopPageUsageItemViewModel
            {
                PageName          = g.Key,
                Count             = g.Count(),
                AverageDurationMs = g.Average(x => x.DurationMs),
            })
            .ToList();

        var vm = new UsageStatisticIndexViewModel
        {
            DateFrom          = startDate,
            DateTo            = endDate,
            Keyword           = keyword,
            TotalRequests     = total,
            SuccessRequests   = success,
            FailedRequests    = total - success,
            AverageDurationMs = avgMs,
            DailyUsage        = daily,
            TopPages          = topPages,
            LatestLogs        = logs.Take(100).ToList(),
        };

        return View(vm);
    }
}
