using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;

namespace Needis.Web.Middleware;

public class SeoRedirectMiddleware
{
    private readonly RequestDelegate _next;

    // Static file prefixes to skip — no DB hit for assets
    private static readonly string[] SkipPrefixes =
        ["/css", "/js", "/lib", "/images", "/uploads", "/favicon", "/_framework", "/_blazor"];

    public SeoRedirectMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (!ShouldCheck(path))
        {
            await _next(context);
            return;
        }

        try
        {
            using var scope = context.RequestServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var redirect = await db.SeoRedirects.AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.SourcePath == path &&
                    r.IsActive   &&
                    !r.IsDelete);

            if (redirect is not null)
            {
                context.Response.StatusCode = redirect.StatusCode;
                context.Response.Headers.Location = redirect.TargetPath;
                return;
            }
        }
        catch
        {
            // Do not crash the site if redirect lookup fails
        }

        await _next(context);
    }

    private static bool ShouldCheck(string path)
    {
        if (string.IsNullOrEmpty(path) || path == "/") return false;
        foreach (var prefix in SkipPrefixes)
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }
}
