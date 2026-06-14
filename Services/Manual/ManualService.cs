using Markdig;
using Needis.Web.ViewModels.Manual;

namespace Needis.Web.Services.Manual;

public class ManualService : IManualService
{
    private readonly IWebHostEnvironment _env;
    private readonly MarkdownPipeline    _pipeline;

    // ── Keys & file mapping ────────────────────────────────────────────────
    public const string DefaultKey = "customer-manual";

    private static readonly Dictionary<string, string> FileMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["customer-manual"]   = "CUSTOMER_USER_MANUAL.md",
            ["admin-manual"]      = "ADMIN_MANUAL.md",
            ["uat-guide"]         = "TESTING_GUIDE.md",
            ["deployment-manual"] = "DEPLOYMENT_MANUAL.md",
            ["user-manual"]       = "USER_MANUAL.md",
        };

    private static readonly Dictionary<string, string> TitleMapTH =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["customer-manual"]   = "คู่มือสำหรับลูกค้า",
            ["admin-manual"]      = "คู่มือผู้ดูแลระบบ",
            ["uat-guide"]         = "คู่มือการทดสอบ UAT",
            ["deployment-manual"] = "คู่มือ Deploy",
            ["user-manual"]       = "คู่มือผู้ใช้งาน",
        };

    private static readonly Dictionary<string, string> TitleMapEN =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["customer-manual"]   = "Customer Manual",
            ["admin-manual"]      = "Admin Manual",
            ["uat-guide"]         = "Testing Guide",
            ["deployment-manual"] = "Deployment Manual",
            ["user-manual"]       = "User Manual",
        };

    public ManualService(IWebHostEnvironment env)
    {
        _env = env;
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    // ── Public API ─────────────────────────────────────────────────────────

    public bool ManualExists(string? manualKey)
    {
        if (string.IsNullOrWhiteSpace(manualKey)) return false;
        return FileMap.ContainsKey(manualKey.Trim()) && FindFilePath(manualKey.Trim()) is not null;
    }

    public async Task<string> GetManualMarkdownAsync(string? manualKey)
    {
        if (string.IsNullOrWhiteSpace(manualKey)) return "";
        var key = manualKey.Trim();
        if (!FileMap.ContainsKey(key)) return "";
        var path = FindFilePath(key);
        if (path is null) return "";
        return await File.ReadAllTextAsync(path);
    }

    public async Task<string> GetManualHtmlAsync(string? manualKey)
    {
        var markdown = await GetManualMarkdownAsync(manualKey);
        if (string.IsNullOrEmpty(markdown))
            return "<p class=\"text-muted fst-italic\">ไม่พบเนื้อหาคู่มือ (Manual file not found)</p>";
        return Markdown.ToHtml(markdown, _pipeline);
    }

    public List<ManualMenuItemViewModel> GetAdminManualMenu() =>
    [
        new() { Key = "customer-manual",   TitleTH = "คู่มือลูกค้า",    TitleEN = "Customer Manual",   Url = "/Admin/Manual/Customer",   IconCss = "bi-person-lines-fill" },
        new() { Key = "admin-manual",      TitleTH = "คู่มือ Admin",     TitleEN = "Admin Manual",      Url = "/Admin/Manual/Admin",      IconCss = "bi-shield-lock" },
        new() { Key = "uat-guide",         TitleTH = "คู่มือทดสอบ UAT", TitleEN = "Testing Guide",     Url = "/Admin/Manual/Testing",    IconCss = "bi-clipboard2-check" },
        new() { Key = "deployment-manual", TitleTH = "คู่มือ Deploy",   TitleEN = "Deployment Manual", Url = "/Admin/Manual/Deployment", IconCss = "bi-cloud-upload" },
    ];

    public List<ManualMenuItemViewModel> GetCustomerManualMenu() =>
    [
        new() { Key = "customer-manual", TitleTH = "คู่มือการใช้งาน", TitleEN = "User Manual", Url = "/Manual", IconCss = "bi-book" },
    ];

    public string GetTitle(string? manualKey, bool thFirst = true)
    {
        if (string.IsNullOrWhiteSpace(manualKey)) manualKey = DefaultKey;
        return thFirst
            ? TitleMapTH.GetValueOrDefault(manualKey, manualKey)
            : TitleMapEN.GetValueOrDefault(manualKey, manualKey);
    }

    // ── Path resolution ────────────────────────────────────────────────────
    // docs/ lives inside Needis.Web/ and is copied to output by the csproj.
    // We try several candidate directories to cover:
    //   - Published / production  : ContentRootPath/docs/
    //   - Dev (run from proj dir) : ContentRootPath/docs/
    //   - Dev (run from solution root, where CWD != proj dir):
    //       AppContext.BaseDirectory is bin/Debug/net10.0/ → 3 levels up = Needis.Web/

    private string? FindFilePath(string manualKey)
    {
        if (!FileMap.TryGetValue(manualKey, out var fileName)) return null;
        foreach (var dir in CandidateDocsDirs())
        {
            var path = Path.Combine(dir, fileName);
            if (File.Exists(path)) return path;
        }
        return null;
    }

    private IEnumerable<string> CandidateDocsDirs()
    {
        var contentRoot = _env.ContentRootPath.TrimEnd(Path.DirectorySeparatorChar);
        var cwd         = Directory.GetCurrentDirectory().TrimEnd(Path.DirectorySeparatorChar);
        var baseDir     = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);

        return new[]
        {
            // Published / dev from project dir → ContentRootPath/docs
            Path.GetFullPath(Path.Combine(contentRoot, "docs")),
            // Dev from solution root → CWD/Needis.Web/docs
            Path.GetFullPath(Path.Combine(cwd, "Needis.Web", "docs")),
            // CWD itself has a docs child (CWD = project dir)
            Path.GetFullPath(Path.Combine(cwd, "docs")),
            // From bin/Debug/net10.0 → 3 levels up = Needis.Web/docs  ← KEY FIX
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "docs")),
            // From bin/Debug/net10.0 → 4 levels up = solution root/docs (legacy)
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "docs")),
        }.Distinct();
    }
}
