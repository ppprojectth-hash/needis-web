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
    public Task<IActionResult> Customer() => RenderPage("customer");

    // GET /Admin/Manual/Admin
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Admin() => RenderPage("admin");

    // GET /Admin/Manual/Testing
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Testing() => RenderPage("testing");

    // GET /Admin/Manual/Deployment
    [HttpGet]
    [RequirePermission("Manual.View")]
    public Task<IActionResult> Deployment() => RenderPage("deployment");

    // GET /Admin/Manual/Print/{manualKey} — print-friendly, no Admin sidebar
    [HttpGet]
    [RequirePermission("Manual.View")]
    public async Task<IActionResult> Print(string manualKey)
    {
        if (!_manual.ManualExists(manualKey))
            return NotFound();

        var html = await _manual.GetManualHtmlAsync(manualKey);
        var vm   = BuildVm(manualKey, html, isAdmin: true);
        return View("Print", vm);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

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
            "customer"   => "คู่มือสำหรับลูกค้า",
            "admin"      => "คู่มือผู้ดูแลระบบ",
            "testing"    => "คู่มือทดสอบ UAT",
            "deployment" => "คู่มือ Deploy",
            _            => "คู่มือ",
        },
        HtmlContent = html,
        IsAdminPage = isAdmin,
        MenuItems   = _manual.GetAdminManualMenu()
            .Select(m => new ManualMenuItemViewModel
            {
                Key     = m.Key,
                TitleTH = m.TitleTH,
                TitleEN = m.TitleEN,
                Url     = m.Url,
                IconCss = m.IconCss,
                IsActive = string.Equals(m.Key, key, StringComparison.OrdinalIgnoreCase),
            })
            .ToList(),
    };
}
