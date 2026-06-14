namespace Needis.Web.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int TotalProducts          { get; set; }
    public int ActiveProducts         { get; set; }
    public int TotalCategories        { get; set; }
    public int TotalServices          { get; set; }
    public int ActiveServices         { get; set; }
    public int TotalActivities        { get; set; }
    public int PublishedActivities    { get; set; }
    public int NewQuotationRequests   { get; set; }
    public int TotalQuotationRequests { get; set; }
    public int NewContactMessages     { get; set; }
    public int TotalContactMessages   { get; set; }
    public int TodayUsageCount        { get; set; }
    public int FailedUsageCount       { get; set; }
    public int EmailSuccessCount      { get; set; }
    public int EmailFailedCount       { get; set; }
    public int EmailSkippedCount      { get; set; }

    public List<DashboardQuotationItemViewModel>     LatestQuotationRequests { get; set; } = new();
    public List<DashboardContactMessageItemViewModel> LatestContactMessages  { get; set; } = new();
    public List<DashboardTopPageItemViewModel>        TopPages                { get; set; } = new();
    public List<DashboardTopProductItemViewModel>     TopProducts             { get; set; } = new();
    public List<DashboardDailyUsageItemViewModel>     DailyUsage              { get; set; } = new();
}

public class DashboardQuotationItemViewModel
{
    public int      Id           { get; set; }
    public string   RequestNo    { get; set; } = string.Empty;
    public string?  CustomerName { get; set; }
    public string?  CompanyName  { get; set; }
    public string?  Email        { get; set; }
    public string?  Subject      { get; set; }
    public string?  RequestType  { get; set; }
    public string   Status       { get; set; } = string.Empty;
    public DateTime CreatedAt    { get; set; }
}

public class DashboardContactMessageItemViewModel
{
    public int      Id        { get; set; }
    public string?  FullName  { get; set; }
    public string?  Email     { get; set; }
    public string?  Subject   { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class DashboardTopPageItemViewModel
{
    public string? PageName { get; set; }
    public string? Url      { get; set; }
    public int     Count    { get; set; }
}

public class DashboardTopProductItemViewModel
{
    public int     ProductId     { get; set; }
    public string? ProductNameEN { get; set; }
    public string? ProductNameTH { get; set; }
    public string? Slug          { get; set; }
    public int     ViewCount     { get; set; }
}

public class DashboardDailyUsageItemViewModel
{
    public DateTime Date  { get; set; }
    public int      Count { get; set; }
}
