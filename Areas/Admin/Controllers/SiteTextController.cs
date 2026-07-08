using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Authorization;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("SiteText.View")]
public class SiteTextController : Controller
{
    private readonly AppDbContext _db;
    private string CurrentUser => User.Identity?.Name ?? "system";

    public SiteTextController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword, string? page)
    {
        ViewData["Title"] = "Website Texts";
        ViewBag.Keyword   = keyword;
        ViewBag.Page      = page;

        ViewBag.Pages = await _db.SiteTexts.AsNoTracking()
            .Where(t => !t.IsDelete)
            .Select(t => t.Page)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();

        var query = _db.SiteTexts.AsNoTracking()
            .Where(t => !t.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(page))
            query = query.Where(t => t.Page == page);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(t =>
                t.Key.Contains(kw) ||
                t.Page.Contains(kw) ||
                (t.Section != null && t.Section.Contains(kw)) ||
                t.Label.Contains(kw) ||
                (t.TextTH != null && t.TextTH.Contains(kw)) ||
                (t.TextEN != null && t.TextEN.Contains(kw)));
        }

        var list = await query
            .OrderBy(t => t.Page)
            .ThenBy(t => t.Section)
            .ThenBy(t => t.DisplayOrder)
            .ThenBy(t => t.Key)
            .ToListAsync();

        return View(list);
    }

    [HttpGet]
    [RequirePermission("SiteText.Edit")]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add Website Text";
        return View(new SiteTextViewModel { IsNew = true, IsActive = true, TextType = "text" });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission("SiteText.Edit")]
    public async Task<IActionResult> Create(SiteTextViewModel vm)
    {
        ViewData["Title"] = "Add Website Text";
        vm.IsNew = true;

        if (await _db.SiteTexts.AsNoTracking().AnyAsync(t => t.Key == vm.Key))
            ModelState.AddModelError(nameof(vm.Key), "This key already exists.");

        if (!ModelState.IsValid) return View(vm);

        _db.SiteTexts.Add(new SiteText
        {
            Key          = vm.Key.Trim(),
            Page         = vm.Page.Trim(),
            Section      = vm.Section,
            Label        = vm.Label,
            TextTH       = vm.TextTH,
            TextEN       = vm.TextEN,
            Description  = vm.Description,
            TextType     = vm.TextType,
            DisplayOrder = vm.DisplayOrder,
            IsActive     = vm.IsActive,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy    = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Website text added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Website Text";
        var t = await _db.SiteTexts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (t is null) return NotFound();

        return View(new SiteTextViewModel
        {
            Id           = t.Id,
            Key          = t.Key,
            Page         = t.Page,
            Section      = t.Section,
            Label        = t.Label,
            TextTH       = t.TextTH,
            TextEN       = t.TextEN,
            Description  = t.Description,
            TextType     = t.TextType,
            DisplayOrder = t.DisplayOrder,
            IsActive     = t.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [RequirePermission("SiteText.Edit")]
    public async Task<IActionResult> Edit(int id, SiteTextViewModel vm)
    {
        ViewData["Title"] = "Edit Website Text";
        ModelState.Remove(nameof(vm.Key));

        if (!ModelState.IsValid) return View(vm);

        var entity = await _db.SiteTexts.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        vm.Key = entity.Key; // key is immutable after create

        entity.Page         = vm.Page.Trim();
        entity.Section      = vm.Section;
        entity.Label        = vm.Label;
        entity.TextTH       = vm.TextTH;
        entity.TextEN       = vm.TextEN;
        entity.Description  = vm.Description;
        entity.TextType     = vm.TextType;
        entity.DisplayOrder = vm.DisplayOrder;
        entity.IsActive     = vm.IsActive;
        entity.UpdatedAtUtc  = DateTime.UtcNow;
        entity.UpdatedBy     = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Website text updated.";
        return RedirectToAction(nameof(Index));
    }
}
