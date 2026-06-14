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
[RequirePermission("Activity.Edit")]
public class ActivityImageController : Controller
{
    private readonly AppDbContext        _db;
    private readonly IWebHostEnvironment _env;

    public ActivityImageController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(int activityId, int? blockId)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == activityId && !x.IsDelete);
        if (activity is null) return NotFound();

        ViewBag.Activity  = activity;
        ViewBag.BlockId   = blockId;
        ViewData["Title"] = $"Images — {activity.ActivityTitleEN}";

        var query = _db.ActivityImages.AsNoTracking()
            .Where(i => i.ActivityId == activityId && !i.IsDelete);

        if (blockId.HasValue)
            query = query.Where(i => i.ActivityDetailBlockId == blockId.Value);

        var list = await query.OrderBy(i => i.DisplayOrder).ToListAsync();
        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Create(int activityId, int? blockId)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == activityId && !x.IsDelete);
        if (activity is null) return NotFound();

        ViewBag.Activity  = activity;
        ViewData["Title"] = "Add Image";

        return View(new ActivityImageViewModel
        {
            ActivityId            = activityId,
            ActivityDetailBlockId = blockId,
            IsActive              = true,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityImageViewModel vm)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ActivityId && !x.IsDelete);
        ViewBag.Activity  = activity;
        ViewData["Title"] = "Add Image";

        if (vm.ImageFile is null || vm.ImageFile.Length == 0)
            ModelState.AddModelError(nameof(vm.ImageFile), "Image file is required.");

        if (!ModelState.IsValid)
            return View(vm);

        var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile!, _env.WebRootPath, "uploads/activity/albums");
        if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }

        // If this is the cover, clear IsCover from others
        if (vm.IsCover)
        {
            var others = await _db.ActivityImages
                .Where(i => i.ActivityId == vm.ActivityId && i.IsCover && !i.IsDelete)
                .ToListAsync();
            foreach (var img in others)
                img.IsCover = false;
        }

        _db.ActivityImages.Add(new ActivityImage
        {
            ActivityId            = vm.ActivityId,
            ActivityDetailBlockId = vm.ActivityDetailBlockId,
            ImageUrl              = path,
            ImageTitleTH          = vm.ImageTitleTH,
            ImageTitleEN          = vm.ImageTitleEN,
            CaptionTH             = vm.CaptionTH,
            CaptionEN             = vm.CaptionEN,
            AltTextTH             = vm.AltTextTH,
            AltTextEN             = vm.AltTextEN,
            ImageType             = vm.ImageType,
            DisplayOrder          = vm.DisplayOrder,
            IsCover               = vm.IsCover,
            IsActive              = vm.IsActive,
            CreatedAt             = DateTime.UtcNow,
            CreatedBy             = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Image added.";
        return RedirectToAction(nameof(Index), new { activityId = vm.ActivityId, blockId = vm.ActivityDetailBlockId });
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Image";

        var img = await _db.ActivityImages.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (img is null) return NotFound();

        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == img.ActivityId && !x.IsDelete);
        ViewBag.Activity = activity;

        return View(new ActivityImageViewModel
        {
            Id                    = img.Id,
            ActivityId            = img.ActivityId,
            ActivityDetailBlockId = img.ActivityDetailBlockId,
            ExistingImageUrl      = img.ImageUrl,
            ImageTitleTH          = img.ImageTitleTH,
            ImageTitleEN          = img.ImageTitleEN,
            CaptionTH             = img.CaptionTH,
            CaptionEN             = img.CaptionEN,
            AltTextTH             = img.AltTextTH,
            AltTextEN             = img.AltTextEN,
            ImageType             = img.ImageType,
            DisplayOrder          = img.DisplayOrder,
            IsCover               = img.IsCover,
            IsActive              = img.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActivityImageViewModel vm)
    {
        ViewData["Title"] = "Edit Image";

        var entity = await _db.ActivityImages.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == entity.ActivityId && !x.IsDelete);
        ViewBag.Activity = activity;

        if (!ModelState.IsValid)
            return View(vm);

        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/activity/albums");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            entity.ImageUrl = path;
        }

        // If setting as cover, clear others
        if (vm.IsCover && !entity.IsCover)
        {
            var others = await _db.ActivityImages
                .Where(i => i.ActivityId == entity.ActivityId && i.IsCover && !i.IsDelete && i.Id != id)
                .ToListAsync();
            foreach (var img in others)
                img.IsCover = false;
        }

        entity.ImageTitleTH  = vm.ImageTitleTH;
        entity.ImageTitleEN  = vm.ImageTitleEN;
        entity.CaptionTH     = vm.CaptionTH;
        entity.CaptionEN     = vm.CaptionEN;
        entity.AltTextTH     = vm.AltTextTH;
        entity.AltTextEN     = vm.AltTextEN;
        entity.ImageType     = vm.ImageType;
        entity.DisplayOrder  = vm.DisplayOrder;
        entity.IsCover       = vm.IsCover;
        entity.IsActive      = vm.IsActive;
        entity.UpdatedAt     = DateTime.UtcNow;
        entity.UpdatedBy     = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Image updated.";
        return RedirectToAction(nameof(Index), new { activityId = entity.ActivityId, blockId = entity.ActivityDetailBlockId });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ActivityImages.FindAsync(id);
        if (entity is not null)
        {
            var activityId = entity.ActivityId;
            var blockId    = entity.ActivityDetailBlockId;
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Image deleted.";
            return RedirectToAction(nameof(Index), new { activityId, blockId });
        }
        return RedirectToAction(nameof(Index), new { activityId = 0 });
    }
}
