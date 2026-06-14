using Markdig;
using Needis.Web.ViewModels.Manual;

namespace Needis.Web.Services.Manual;

public class ManualService : IManualService
{
    private readonly IWebHostEnvironment _env;
    private readonly MarkdownPipeline   _pipeline;

    // Maps manualKey → docs-relative file name
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

    public bool ManualExists(string manualKey) =>
        FileMap.ContainsKey(manualKey) && File.Exists(ResolvePath(manualKey));

    public async Task<string> GetManualMarkdownAsync(string manualKey)
    {
        if (!FileMap.ContainsKey(manualKey)) return "";
        var path = ResolvePath(manualKey);
        if (!File.Exists(path)) return "";
        return await File.ReadAllTextAsync(path);
    }

    public async Task<string> GetManualHtmlAsync(string manualKey)
    {
        var markdown = await GetManualMarkdownAsync(manualKey);
        if (string.IsNullOrEmpty(markdown))
            return "<p class=\"text-muted\">ไม่พบเนื้อหาคู่มือ</p>";
        return Markdown.ToHtml(markdown, _pipeline);
    }

    public List<ManualMenuItemViewModel> GetAdminManualMenu() =>
    [
        new() { Key = "customer",   TitleTH = "คู่มือลูกค้า",       TitleEN = "Customer Manual",  Url = "/Admin/Manual/Customer",   IconCss = "bi-person-lines-fill" },
        new() { Key = "admin",      TitleTH = "คู่มือ Admin",        TitleEN = "Admin Manual",     Url = "/Admin/Manual/Admin",      IconCss = "bi-shield-lock" },
        new() { Key = "testing",    TitleTH = "คู่มือทดสอบ UAT",     TitleEN = "Testing Guide",    Url = "/Admin/Manual/Testing",    IconCss = "bi-clipboard2-check" },
        new() { Key = "deployment", TitleTH = "คู่มือ Deploy",       TitleEN = "Deployment Manual",Url = "/Admin/Manual/Deployment", IconCss = "bi-cloud-upload" },
    ];

    public List<ManualMenuItemViewModel> GetCustomerManualMenu() =>
    [
        new() { Key = "customer", TitleTH = "คู่มือการใช้งาน", TitleEN = "User Manual", Url = "/Manual", IconCss = "bi-book" },
    ];

    public string GetTitle(string manualKey, bool thFirst = true) =>
        thFirst
            ? TitleMapTH.GetValueOrDefault(manualKey, manualKey)
            : TitleMapEN.GetValueOrDefault(manualKey, manualKey);

    private string ResolvePath(string manualKey)
    {
        var fileName = FileMap[manualKey];
        // docs/ is sibling of Needis.Web/ — go up from ContentRootPath
        var contentRoot = _env.ContentRootPath;
        var docsDir = Path.Combine(contentRoot, "..", "docs");
        return Path.GetFullPath(Path.Combine(docsDir, fileName));
    }
}
