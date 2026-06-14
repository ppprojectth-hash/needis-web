using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services.Manual;
using Needis.Web.ViewModels.Manual;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ManualController : Controller
{
    private readonly IManualService _manual;

    public ManualController(IManualService manual) => _manual = manual;

    // GET /Admin/Manual → redirect to Customer manual
    [RequirePermission("Manual.View")]
    public IActionResult Index() => RedirectToAction(nameof(Customer));

    // GET /Admin/Manual/Customer
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Customer() => RenderPage("customer-manual");

    // GET /Admin/Manual/Admin
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Admin() => RenderPage("admin-manual");

    // GET /Admin/Manual/Testing
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Testing() => RenderPage("uat-guide");

    // GET /Admin/Manual/Deployment
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Deployment() => RenderPage("deployment-manual");

    // GET /Admin/Manual/Print/{manualKey}
    [HttpGet]
    [RequirePermission("Manual.View")]
    public async Task<IActionResult> Print(string? manualKey)
    {
        var key  = NormalizeKey(manualKey);
        var html = await _manual.GetManualHtmlAsync(key);
        var vm   = BuildVm(key, html, isAdmin: true);
        return View("Print", vm);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private string NormalizeKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)) return ManualService.DefaultKey;
        key = key.Trim();
        return _manual.ManualExists(key) ? key : ManualService.DefaultKey;
    }

    private async Task<IActionResult> RenderPage(string key)
    {
        var html = await _manual.GetManualHtmlAsync(key);
        var vm   = BuildVm(key, html, isAdmin: true);
        ViewData["Title"] = $"Manual — {vm.Title}";
        return View("ManualPage", vm);
    }

    private ManualPageViewModel BuildVm(string key, string html, bool isAdmin) => new()
    {
        ManualKey   = key,
        Title       = key switch
        {
            "customer-manual"   => "คู่มือสำหรับลูกค้า",
            "admin-manual"      => "คู่มือผู้ดูแลระบบ",
            "uat-guide"         => "คู่มือทดสอบ UAT",
            "deployment-manual" => "คู่มือ Deploy",
            _                   => "คู่มือ",
        },
        HtmlContent = html,
        IsAdminPage = isAdmin,
        MenuItems   = _manual.GetAdminManualMenu()
            .Select(m => new ManualMenuItemViewModel
            {
                Key      = m.Key,
                TitleTH  = m.TitleTH,
                TitleEN  = m.TitleEN,
                Url      = m.Url,
                IconCss  = m.IconCss,
                IsActive = string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase),
            })
            .ToList(),
    };
}
