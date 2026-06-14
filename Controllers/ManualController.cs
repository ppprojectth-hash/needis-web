using Microsoft.AspNetCore.Mvc;
using Needis.Web.Services.Manual;
using Needis.Web.ViewModels.Manual;

namespace Needis.Web.Controllers;

// Public controller — only exposes the CUSTOMER manual.
// Admin / Deployment / Testing manuals are NOT accessible here.
public class ManualController : Controller
{
    private readonly IManualService _manual;

    public ManualController(IManualService manual) => _manual = manual;

    // GET /Manual
    public async Task<IActionResult> Index()
    {
        var html = await _manual.GetManualHtmlAsync("customer");
        var vm   = BuildVm("customer", html);
        ViewData["Title"] = "คู่มือการใช้งาน — User Manual";
        return View(vm);
    }

    // GET /Manual/Print
    [HttpGet]
    public async Task<IActionResult> Print()
    {
        var html = await _manual.GetManualHtmlAsync("customer");
        var vm   = BuildVm("customer", html);
        ViewData["Title"] = "คู่มือการใช้งาน — พิมพ์";
        return View(vm);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private ManualPageViewModel BuildVm(string key, string html) => new()
    {
        ManualKey   = key,
        Title       = "คู่มือสำหรับลูกค้า",
        HtmlContent = html,
        IsAdminPage = false,
        MenuItems   = _manual.GetCustomerManualMenu(),
    };
}
