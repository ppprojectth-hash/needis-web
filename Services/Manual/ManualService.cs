using Markdig;
using Needis.Web.ViewModels.Manual;

namespace Needis.Web.Services.Manual;

public class ManualService : IManualService
{
    private readonly IWebHostEnvironment _env;
    private readonly MarkdownPipeline   _pipeline;

    private static readonly Dictionary<string, string> FileMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["customer"]   = "CUSTOMER_USER_MANUAL.md",
        ["admin"]      = "ADMIN_MANUAL.md",
        ["testing"]    = "TESTING_GUIDE.md",
        ["deployment"] = "DEPLOYMENT_MANUAL.md",
    };

    private static readonly Dictionary<string, string> TitleMapTH = new(StringComparer.OrdinalIgnoreCase)
    {
        ["customer"]   = "คู่มือสำหรับลูกค้า",
        ["admin"]      = "คู่มือผู้ดูแลระบบ",
        ["testing"]    = "คู่มือการทดสอบ UAT",
        ["deployment"] = "คู่มือ Deploy",
    };

    private static readonly Dictionary<string, string> TitleMapEN = new(StringComparer.OrdinalIgnoreCase)
    {
        ["customer"]   = "Customer Manual",
        ["admin"]      = "Admin Manual",
        ["testing"]    = "Testing Guide",
        ["deployment"] = "Deployment Manual",
    };

    public ManualService(IWebHostEnvironment env)
    {
        _env = env;
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();
    }

    private const string DefaultKey = "customer";

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
            return "<p class=\"text-muted\">ไม่พบเนื้อหาคู่มือ (Manual file not found)</p>";
        return Markdown.ToHtml(markdown, _pipeline);
    }

    public List<ManualMenuItemViewModel> GetAdminManualMenu() =>
    [
        new() { Key = "customer",   TitleTH = "คู่มือลูกค้า",    TitleEN = "Customer Manual",  Url = "/Admin/Manual/Customer",   IconCss = "bi-person-lines-fill" },
        new() { Key = "admin",      TitleTH = "คู่มือ Admin",     TitleEN = "Admin Manual",     Url = "/Admin/Manual/Admin",      IconCss = "bi-shield-lock" },
        new() { Key = "testing",    TitleTH = "คู่มือทดสอบ UAT", TitleEN = "Testing Guide",    Url = "/Admin/Manual/Testing",    IconCss = "bi-clipboard2-check" },
        new() { Key = "deployment", TitleTH = "คู่มือ Deploy",   TitleEN = "Deployment Manual", Url = "/Admin/Manual/Deployment", IconCss = "bi-cloud-upload" },
    ];

    public List<ManualMenuItemViewModel> GetCustomerManualMenu() =>
    [
        new() { Key = "customer", TitleTH = "คู่มือการใช้งาน", TitleEN = "User Manual", Url = "/Manual", IconCss = "bi-book" },
    ];

    public string GetTitle(string manualKey, bool thFirst = true) =>
        thFirst
            ? TitleMapTH.GetValueOrDefault(manualKey, manualKey)
            : TitleMapEN.GetValueOrDefault(manualKey, manualKey);

    // ── Path resolution ──────────────────────────────────────────────────────

    // Returns the full path to the markdown file, or null if not found.
    // Tries several candidate directories because ContentRootPath varies
    // depending on whether the app is started from the solution root or
    // the project directory.
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
        var contentRoot = _env.ContentRootPath;
        var cwd         = Directory.GetCurrentDirectory();
        var baseDir     = AppContext.BaseDirectory; // e.g. bin/Debug/net10.0

        // Normalise: strip trailing slash
        contentRoot = contentRoot.TrimEnd(Path.DirectorySeparatorChar);
        cwd         = cwd.TrimEnd(Path.DirectorySeparatorChar);
        baseDir     = baseDir.TrimEnd(Path.DirectorySeparatorChar);

        return new[]
        {
            // When CWD == solution root (most common with "dotnet run --project Needis.Web")
            Path.GetFullPath(Path.Combine(cwd, "docs")),
            // When CWD == project dir  (dotnet run inside Needis.Web/)
            Path.GetFullPath(Path.Combine(cwd, "..", "docs")),
            // ContentRoot == solution root
            Path.GetFullPath(Path.Combine(contentRoot, "docs")),
            // ContentRoot == project dir (Needis.Web/)
            Path.GetFullPath(Path.Combine(contentRoot, "..", "docs")),
            // From bin/Debug/net10.0 — go up 4 levels to solution root
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "docs")),
            // From bin/Debug/net10.0 — go up 3 levels to Needis.Web/, then sibling docs/
            Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", "docs")),
        }.Distinct();
    }
}
