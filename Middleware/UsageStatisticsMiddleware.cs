using System.Diagnostics;
using System.Security.Claims;
using Needis.Web.Data;
using Needis.Web.Models;

namespace Needis.Web.Middleware;

public class UsageStatisticsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UsageStatisticsMiddleware> _logger;

    private static readonly string[] SkipPrefixes =
    [
        "/css", "/js", "/lib", "/images", "/uploads", "/favicon",
        "/robots.txt", "/_framework", "/_vs", "/health",
    ];

    public UsageStatisticsMiddleware(
        RequestDelegate next,
        IServiceScopeFactory scopeFactory,
        ILogger<UsageStatisticsMiddleware> logger)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "/";

        if (ShouldSkip(path))
        {
            await _next(context);
            return;
        }

        var sw = Stopwatch.StartNew();

        try
        {
            await _next(context);
            sw.Stop();
            await LogAsync(context, path, context.Response.StatusCode, sw.ElapsedMilliseconds, null);
        }
        catch (Exception ex)
        {
            sw.Stop();
            await LogAsync(context, path, 500, sw.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }

    private static bool ShouldSkip(string path)
    {
        foreach (var prefix in SkipPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private async Task LogAsync(HttpContext context, string path, int statusCode, long durationMs, string? errorMessage)
    {
        try
        {
            var route  = context.GetRouteData()?.Values;
            var area   = route?["area"]?.ToString();
            var ctrl   = route?["controller"]?.ToString();
            var action = route?["action"]?.ToString();

            var pageName = (ctrl is not null || action is not null)
                ? (area is not null ? $"{area}/{ctrl}/{action}" : $"{ctrl}/{action}")
                : null;

            var functionName = (ctrl is not null && action is not null)
                ? $"{ctrl}.{action}"
                : null;

            var userId   = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var username = context.User.FindFirst(ClaimTypes.Name)?.Value;

            var ip        = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers.UserAgent.FirstOrDefault();
            var referrer  = context.Request.Headers.Referer.FirstOrDefault();

            var lang = context.Request.Cookies["Needis.Language"]
                ?? context.Request.Query["lang"].FirstOrDefault()
                ?? "en";
            if (lang != "th" && lang != "en") lang = "en";

            var qs      = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null;
            var fullUrl = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.Path}{context.Request.QueryString}";

            var log = new UsageLog
            {
                PageUrl      = Truncate(fullUrl, 500) ?? string.Empty,
                Path         = Truncate(path, 500),
                QueryString  = Truncate(qs, 1000),
                HttpMethod   = context.Request.Method,
                Area         = Truncate(area, 50),
                Controller   = Truncate(ctrl, 100),
                Action       = Truncate(action, 100),
                PageName     = Truncate(pageName, 200),
                FunctionName = Truncate(functionName, 200),
                UserId       = Truncate(userId, 100),
                Username     = Truncate(username, 200),
                IpAddress    = Truncate(ip, 50),
                UserAgent    = Truncate(userAgent, 500),
                Referrer     = Truncate(referrer, 500),
                Language     = lang,
                StatusCode   = statusCode,
                IsSuccess    = statusCode >= 200 && statusCode <= 399,
                ErrorMessage = Truncate(errorMessage, 2000),
                DurationMs   = durationMs,
                AccessedAt   = DateTime.UtcNow,
            };

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.UsageLogs.Add(log);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record usage log for {Path}", path);
        }
    }

    private static string? Truncate(string? s, int max) =>
        s is null ? null : (s.Length > max ? s[..max] : s);
}
