using Needis.Web.Models;

namespace Needis.Web.ViewModels.Admin;

public class DailyUsageItemViewModel
{
    public DateOnly Date              { get; init; }
    public int      TotalRequests     { get; init; }
    public int      SuccessRequests   { get; init; }
    public int      FailedRequests    { get; init; }
    public double   AverageDurationMs { get; init; }
}

public class TopPageUsageItemViewModel
{
    public string? PageName          { get; init; }
    public int     Count             { get; init; }
    public double  AverageDurationMs { get; init; }
}

public class UsageStatisticIndexViewModel
{
    public DateTime DateFrom          { get; init; }
    public DateTime DateTo            { get; init; }
    public string?  Keyword           { get; init; }
    public int      TotalRequests     { get; init; }
    public int      SuccessRequests   { get; init; }
    public int      FailedRequests    { get; init; }
    public double   AverageDurationMs { get; init; }

    public List<DailyUsageItemViewModel>   DailyUsage  { get; init; } = [];
    public List<TopPageUsageItemViewModel> TopPages    { get; init; } = [];
    public List<UsageLog>                  LatestLogs  { get; init; } = [];
}
