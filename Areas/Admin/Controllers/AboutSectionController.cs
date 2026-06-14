using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("About.View")]
public class AboutSectionController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AboutSectionController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewData["Title"]   = "About Sections";
        ViewBag.Keyword     = keyword;

        var query = _db.AboutSections.AsNoTracking()
            .Where(s => !s.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(s =>
                s.SectionKey.Contains(keyword) ||
                (s.TitleTH != null && s.TitleTH.Contains(keyword)) ||
                (s.TitleEN != null && s.TitleEN.Contains(keyword)));

        var list = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.SectionKey)
            .ToListAsync();

        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Create About Section";
        return View(new AboutSectionViewModel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AboutSectionViewModel vm)
    {
        ViewData["Title"] = "Create About Section";

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.AboutSections.AnyAsync(s => s.SectionKey == vm.SectionKey && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.SectionKey), $"SectionKey '{vm.SectionKey}' already exists.");
            return View(vm);
        }

        string? imagePath = null;
        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/sections");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            imagePath = path;
        }

        _db.AboutSections.Add(new AboutSection
        {
            SectionKey    = vm.SectionKey,
            TitleTH       = vm.TitleTH,
            TitleEN       = vm.TitleEN,
            SubtitleTH    = vm.SubtitleTH,
            SubtitleEN    = vm.SubtitleEN,
            DescriptionTH = vm.DescriptionTH,
            DescriptionEN = vm.DescriptionEN,
            ImageUrl      = imagePath,
            LayoutType    = vm.LayoutType,
            DisplayOrder  = vm.DisplayOrder,
            IsActive      = vm.IsActive,
            CreatedAt     = DateTime.UtcNow,
            CreatedBy     = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Section created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit About Section";
        var s = await _db.AboutSections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (s is null) return NotFound();

        return View(new AboutSectionViewModel
        {
            Id               = s.Id,
            SectionKey       = s.SectionKey,
            TitleTH          = s.TitleTH,
            TitleEN          = s.TitleEN,
            SubtitleTH       = s.SubtitleTH,
            SubtitleEN       = s.SubtitleEN,
            DescriptionTH    = s.DescriptionTH,
            DescriptionEN    = s.DescriptionEN,
            ExistingImageUrl = s.ImageUrl,
            LayoutType       = s.LayoutType,
            DisplayOrder     = s.DisplayOrder,
            IsActive         = s.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AboutSectionViewModel vm)
    {
        ViewData["Title"] = "Edit About Section";

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.AboutSections.AnyAsync(s => s.SectionKey == vm.SectionKey && s.Id != id && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.SectionKey), $"SectionKey '{vm.SectionKey}' already exists.");
            return View(vm);
        }

        var entity = await _db.AboutSections.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/sections");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            entity.ImageUrl = path;
        }

        entity.SectionKey    = vm.SectionKey;
        entity.TitleTH       = vm.TitleTH;
        entity.TitleEN       = vm.TitleEN;
        entity.SubtitleTH    = vm.SubtitleTH;
        entity.SubtitleEN    = vm.SubtitleEN;
        entity.DescriptionTH = vm.DescriptionTH;
        entity.DescriptionEN = vm.DescriptionEN;
        entity.LayoutType    = vm.LayoutType;
        entity.DisplayOrder  = vm.DisplayOrder;
        entity.IsActive      = vm.IsActive;
        entity.UpdatedAt     = DateTime.UtcNow;
        entity.UpdatedBy     = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Section updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.AboutSections.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Section deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ── Items ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Items(int sectionId)
    {
        var section = await _db.AboutSections.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDelete);
        if (section is null) return NotFound();

        ViewData["Title"]   = $"Items — {section.TitleEN ?? section.SectionKey}";
        ViewBag.Section     = section;

        var items = await _db.AboutSectionItems.AsNoTracking()
            .Where(i => i.AboutSectionId == sectionId && !i.IsDelete)
            .OrderBy(i => i.DisplayOrder)
            .ToListAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> CreateItem(int sectionId)
    {
        var section = await _db.AboutSections.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDelete);
        if (section is null) return NotFound();

        ViewData["Title"] = "Add Section Item";
        ViewBag.Section   = section;
        return View(new AboutSectionItemViewModel { AboutSectionId = sectionId, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateItem(AboutSectionItemViewModel vm)
    {
        var section = await _db.AboutSections.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == vm.AboutSectionId && !s.IsDelete);
        if (section is null) return NotFound();

        ViewData["Title"] = "Add Section Item";
        ViewBag.Section   = section;

        if (!ModelState.IsValid)
            return View(vm);

        string? imagePath = null;
        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/sections");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            imagePath = path;
        }

        _db.AboutSectionItems.Add(new AboutSectionItem
        {
            AboutSectionId = vm.AboutSectionId,
            TitleTH        = vm.TitleTH,
            TitleEN        = vm.TitleEN,
            DescriptionTH  = vm.DescriptionTH,
            DescriptionEN  = vm.DescriptionEN,
            IconUrl        = vm.IconUrl,
            ImageUrl       = imagePath,
            DisplayOrder   = vm.DisplayOrder,
            IsActive       = vm.IsActive,
            CreatedAt      = DateTime.UtcNow,
            CreatedBy      = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Item added.";
        return RedirectToAction(nameof(Items), new { sectionId = vm.AboutSectionId });
    }

    [HttpGet]
    public async Task<IActionResult> EditItem(int id)
    {
        var item = await _db.AboutSectionItems.AsNoTracking()
            .Include(i => i.AboutSection)
            .FirstOrDefaultAsync(i => i.Id == id && !i.IsDelete);
        if (item is null) return NotFound();

        ViewData["Title"] = "Edit Section Item";
        ViewBag.Section   = item.AboutSection;
        return View(new AboutSectionItemViewModel
        {
            Id               = item.Id,
            AboutSectionId   = item.AboutSectionId,
            TitleTH          = item.TitleTH,
            TitleEN          = item.TitleEN,
            DescriptionTH    = item.DescriptionTH,
            DescriptionEN    = item.DescriptionEN,
            IconUrl          = item.IconUrl,
            ExistingImageUrl = item.ImageUrl,
            DisplayOrder     = item.DisplayOrder,
            IsActive         = item.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(int id, AboutSectionItemViewModel vm)
    {
        var entity = await _db.AboutSectionItems.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        var section = await _db.AboutSections.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == entity.AboutSectionId && !s.IsDelete);

        ViewData["Title"] = "Edit Section Item";
        ViewBag.Section   = section;

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/sections");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            entity.ImageUrl = path;
        }

        entity.TitleTH       = vm.TitleTH;
        entity.TitleEN       = vm.TitleEN;
        entity.DescriptionTH = vm.DescriptionTH;
        entity.DescriptionEN = vm.DescriptionEN;
        entity.IconUrl       = vm.IconUrl;
        entity.DisplayOrder  = vm.DisplayOrder;
        entity.IsActive      = vm.IsActive;
        entity.UpdatedAt     = DateTime.UtcNow;
        entity.UpdatedBy     = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Item updated.";
        return RedirectToAction(nameof(Items), new { sectionId = entity.AboutSectionId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var entity = await _db.AboutSectionItems.FindAsync(id);
        if (entity is not null)
        {
            var sectionId    = entity.AboutSectionId;
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Item deleted.";
            return RedirectToAction(nameof(Items), new { sectionId });
        }
        return RedirectToAction(nameof(Index));
    }
}
