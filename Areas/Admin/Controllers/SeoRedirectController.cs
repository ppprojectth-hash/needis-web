using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SeoRedirectController : Controller
{
    private readonly AppDbContext _db;

    public SeoRedirectController(AppDbContext db) => _db = db;

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Seo.View")]
    public async Task<IActionResult> Index(string? keyword)
    {
        var query = _db.SeoRedirects.AsNoTracking().Where(r => !r.IsDelete);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(r =>
                r.SourcePath.Contains(kw) ||
                r.TargetPath.Contains(kw));
        }

        var items = await query
            .OrderBy(r => r.SourcePath)
            .Select(r => new SeoRedirectListItemViewModel
            {
                Id         = r.Id,
                SourcePath = r.SourcePath,
                TargetPath = r.TargetPath,
                StatusCode = r.StatusCode,
                IsActive   = r.IsActive,
                UpdatedAt  = r.UpdatedAt,
            })
            .ToListAsync();

        ViewBag.Keyword = keyword;
        return View(items);
    }

    // ── Create GET ───────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Seo.Edit")]
    public IActionResult Create() => View(new SeoRedirectEditViewModel());

    // ── Create POST ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Create(SeoRedirectEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        if (!IsValidStatusCode(vm.StatusCode))
        {
            ModelState.AddModelError(nameof(vm.StatusCode), "Status code must be 301 or 302.");
            return View(vm);
        }

        var source = NormalizePath(vm.SourcePath);
        if (await _db.SeoRedirects.AnyAsync(r => r.SourcePath == source && !r.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.SourcePath), "A redirect with this source path already exists.");
            return View(vm);
        }

        var redirect = new SeoRedirect
        {
            SourcePath = source,
            TargetPath = NormalizePath(vm.TargetPath),
            StatusCode = vm.StatusCode,
            IsActive   = vm.IsActive,
            CreatedAt  = DateTime.UtcNow,
            CreatedBy  = User.Identity?.Name,
        };

        _db.SeoRedirects.Add(redirect);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Redirect from '{redirect.SourcePath}' created.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit GET ─────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var r = await _db.SeoRedirects.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (r is null) return NotFound();

        return View(new SeoRedirectEditViewModel
        {
            Id         = r.Id,
            SourcePath = r.SourcePath,
            TargetPath = r.TargetPath,
            StatusCode = r.StatusCode,
            IsActive   = r.IsActive,
        });
    }

    // ── Edit POST ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Edit(int id, SeoRedirectEditViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid) return View(vm);

        if (!IsValidStatusCode(vm.StatusCode))
        {
            ModelState.AddModelError(nameof(vm.StatusCode), "Status code must be 301 or 302.");
            return View(vm);
        }

        var source = NormalizePath(vm.SourcePath);
        if (await _db.SeoRedirects.AnyAsync(r => r.SourcePath == source && r.Id != id && !r.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.SourcePath), "Another redirect with this source path already exists.");
            return View(vm);
        }

        var redirect = await _db.SeoRedirects.FindAsync(id);
        if (redirect is null || redirect.IsDelete) return NotFound();

        redirect.SourcePath = source;
        redirect.TargetPath = NormalizePath(vm.TargetPath);
        redirect.StatusCode = vm.StatusCode;
        redirect.IsActive   = vm.IsActive;
        redirect.UpdatedAt  = DateTime.UtcNow;
        redirect.UpdatedBy  = User.Identity?.Name;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Redirect from '{redirect.SourcePath}' updated.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete POST ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Seo.Edit")]
    public async Task<IActionResult> Delete(int id)
    {
        var redirect = await _db.SeoRedirects.FindAsync(id);
        if (redirect is not null && !redirect.IsDelete)
        {
            redirect.IsDelete  = true;
            redirect.UpdatedAt = DateTime.UtcNow;
            redirect.UpdatedBy = User.Identity?.Name;
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Redirect deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static bool IsValidStatusCode(int code) => code is 301 or 302;

    private static string NormalizePath(string path)
    {
        path = path.Trim();
        if (!path.StartsWith('/')) path = "/" + path;
        return path;
    }
}
