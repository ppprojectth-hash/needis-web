namespace Needis.Web.Helpers;

public static class BangkokTimeHelper
{
    public static readonly TimeZoneInfo BangkokTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Bangkok");

    public static DateTime NowBangkok() =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BangkokTimeZone);

    public static DateTime ConvertBangkokDateStartToUtc(DateTime date)
    {
        var localDate = DateTime.SpecifyKind(date.Date, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localDate, BangkokTimeZone);
    }

    public static DateTime ConvertBangkokDateEndExclusiveToUtc(DateTime date)
    {
        var localDateEndExclusive = DateTime.SpecifyKind(date.Date.AddDays(1), DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localDateEndExclusive, BangkokTimeZone);
    }
}
