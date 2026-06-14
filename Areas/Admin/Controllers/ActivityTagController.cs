using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Activity.View")]
public class ActivityTagController : Controller
{
    private readonly AppDbContext _db;

    public ActivityTagController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    private static string NormalizeTagKey(string input)
    {
        var key = input.Trim().ToLowerInvariant();
        key = Regex.Replace(key, @"\s+", "-");
        key = Regex.Replace(key, @"[^a-z0-9\-]", "");
        key = Regex.Replace(key, @"-{2,}", "-");
        return key.Trim('-');
    }

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewData["Title"] = "Activity Tags";
        ViewBag.Keyword   = keyword;

        var query = _db.ActivityTags.AsNoTracking()
            .Where(t => !t.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(t =>
                t.TagKey.Contains(keyword) ||
                (t.TagNameEN != null && t.TagNameEN.Contains(keyword)) ||
                (t.TagNameTH != null && t.TagNameTH.Contains(keyword)));

        var list = await query
            .OrderBy(t => t.DisplayOrder)
            .ThenBy(t => t.TagKey)
            .ToListAsync();

        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Create Activity Tag";
        return View(new ActivityTagViewModel { IsActive = true, IsFilterable = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityTagViewModel vm)
    {
        ViewData["Title"] = "Create Activity Tag";

        vm.TagKey = NormalizeTagKey(vm.TagKey);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.ActivityTags.AnyAsync(t => t.TagKey == vm.TagKey && !t.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.TagKey), $"TagKey '{vm.TagKey}' already exists.");
            return View(vm);
        }

        _db.ActivityTags.Add(new ActivityTag
        {
            TagKey            = vm.TagKey,
            TagNameTH         = vm.TagNameTH,
            TagNameEN         = vm.TagNameEN,
            TagDescriptionTH  = vm.TagDescriptionTH,
            TagDescriptionEN  = vm.TagDescriptionEN,
            TagColor          = vm.TagColor,
            DisplayOrder      = vm.DisplayOrder,
            IsFilterable      = vm.IsFilterable,
            IsActive          = vm.IsActive,
            CreatedAt         = DateTime.UtcNow,
            CreatedBy         = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Activity tag created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Activity Tag";
        var tag = await _db.ActivityTags.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (tag is null) return NotFound();

        return View(new ActivityTagViewModel
        {
            Id               = tag.Id,
            TagKey           = tag.TagKey,
            TagNameTH        = tag.TagNameTH,
            TagNameEN        = tag.TagNameEN,
            TagDescriptionTH = tag.TagDescriptionTH,
            TagDescriptionEN = tag.TagDescriptionEN,
            TagColor         = tag.TagColor,
            DisplayOrder     = tag.DisplayOrder,
            IsFilterable     = tag.IsFilterable,
            IsActive         = tag.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActivityTagViewModel vm)
    {
        ViewData["Title"] = "Edit Activity Tag";

        vm.TagKey = NormalizeTagKey(vm.TagKey);

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.ActivityTags.AnyAsync(t => t.TagKey == vm.TagKey && t.Id != id && !t.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.TagKey), $"TagKey '{vm.TagKey}' already exists.");
            return View(vm);
        }

        var entity = await _db.ActivityTags.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        entity.TagKey           = vm.TagKey;
        entity.TagNameTH        = vm.TagNameTH;
        entity.TagNameEN        = vm.TagNameEN;
        entity.TagDescriptionTH = vm.TagDescriptionTH;
        entity.TagDescriptionEN = vm.TagDescriptionEN;
        entity.TagColor         = vm.TagColor;
        entity.DisplayOrder     = vm.DisplayOrder;
        entity.IsFilterable     = vm.IsFilterable;
        entity.IsActive         = vm.IsActive;
        entity.UpdatedAt        = DateTime.UtcNow;
        entity.UpdatedBy        = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Activity tag updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ActivityTags.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Activity tag deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
