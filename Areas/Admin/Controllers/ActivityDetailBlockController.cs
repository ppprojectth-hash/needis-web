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
public class ActivityDetailBlockController : Controller
{
    private static readonly string[] AllowedBlockTypes =
        ["heading", "text", "image", "album", "quote", "video", "button"];

    private readonly AppDbContext        _db;
    private readonly IWebHostEnvironment _env;

    public ActivityDetailBlockController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(int activityId)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == activityId && !x.IsDelete);
        if (activity is null) return NotFound();

        ViewBag.Activity  = activity;
        ViewData["Title"] = $"Detail Blocks — {activity.ActivityTitleEN}";

        var list = await _db.ActivityDetailBlocks.AsNoTracking()
            .Where(b => b.ActivityId == activityId && !b.IsDelete)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();

        return View(list);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Create(int activityId)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == activityId && !x.IsDelete);
        if (activity is null) return NotFound();

        ViewBag.Activity      = activity;
        ViewBag.BlockTypes    = AllowedBlockTypes;
        ViewData["Title"]     = "Add Detail Block";

        return View(new ActivityDetailBlockViewModel { ActivityId = activityId, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityDetailBlockViewModel vm)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ActivityId && !x.IsDelete);

        ViewBag.Activity   = activity;
        ViewBag.BlockTypes = AllowedBlockTypes;
        ViewData["Title"]  = "Add Detail Block";

        if (!AllowedBlockTypes.Contains(vm.BlockType))
            ModelState.AddModelError(nameof(vm.BlockType), "Invalid block type.");

        if (!ModelState.IsValid)
            return View(vm);

        string? imagePath = null;
        if (vm.BlockImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BlockImageFile, _env.WebRootPath, "uploads/activity/blocks");
            if (!ok) { ModelState.AddModelError(nameof(vm.BlockImageFile), err); return View(vm); }
            imagePath = path;
        }

        _db.ActivityDetailBlocks.Add(new ActivityDetailBlock
        {
            ActivityId      = vm.ActivityId,
            BlockType       = vm.BlockType,
            BlockTitleTH    = vm.BlockTitleTH,
            BlockTitleEN    = vm.BlockTitleEN,
            BlockSubtitleTH = vm.BlockSubtitleTH,
            BlockSubtitleEN = vm.BlockSubtitleEN,
            BlockContentTH  = vm.BlockContentTH,
            BlockContentEN  = vm.BlockContentEN,
            ImageUrl        = imagePath,
            VideoUrl        = vm.VideoUrl,
            ButtonTextTH    = vm.ButtonTextTH,
            ButtonTextEN    = vm.ButtonTextEN,
            ButtonUrl       = vm.ButtonUrl,
            LayoutType      = vm.LayoutType,
            DisplayOrder    = vm.DisplayOrder,
            IsActive        = vm.IsActive,
            CreatedAt       = DateTime.UtcNow,
            CreatedBy       = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Detail block created.";
        return RedirectToAction(nameof(Index), new { activityId = vm.ActivityId });
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"]  = "Edit Detail Block";
        ViewBag.BlockTypes = AllowedBlockTypes;

        var block = await _db.ActivityDetailBlocks.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (block is null) return NotFound();

        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == block.ActivityId && !x.IsDelete);
        ViewBag.Activity = activity;

        return View(new ActivityDetailBlockViewModel
        {
            Id               = block.Id,
            ActivityId       = block.ActivityId,
            BlockType        = block.BlockType,
            BlockTitleTH     = block.BlockTitleTH,
            BlockTitleEN     = block.BlockTitleEN,
            BlockSubtitleTH  = block.BlockSubtitleTH,
            BlockSubtitleEN  = block.BlockSubtitleEN,
            BlockContentTH   = block.BlockContentTH,
            BlockContentEN   = block.BlockContentEN,
            ExistingImageUrl = block.ImageUrl,
            VideoUrl         = block.VideoUrl,
            ButtonTextTH     = block.ButtonTextTH,
            ButtonTextEN     = block.ButtonTextEN,
            ButtonUrl        = block.ButtonUrl,
            LayoutType       = block.LayoutType,
            DisplayOrder     = block.DisplayOrder,
            IsActive         = block.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActivityDetailBlockViewModel vm)
    {
        ViewData["Title"]  = "Edit Detail Block";
        ViewBag.BlockTypes = AllowedBlockTypes;

        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ActivityId && !x.IsDelete);
        ViewBag.Activity = activity;

        if (!AllowedBlockTypes.Contains(vm.BlockType))
            ModelState.AddModelError(nameof(vm.BlockType), "Invalid block type.");

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _db.ActivityDetailBlocks.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.BlockImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.BlockImageFile, _env.WebRootPath, "uploads/activity/blocks");
            if (!ok) { ModelState.AddModelError(nameof(vm.BlockImageFile), err); return View(vm); }
            entity.ImageUrl = path;
        }

        entity.BlockType       = vm.BlockType;
        entity.BlockTitleTH    = vm.BlockTitleTH;
        entity.BlockTitleEN    = vm.BlockTitleEN;
        entity.BlockSubtitleTH = vm.BlockSubtitleTH;
        entity.BlockSubtitleEN = vm.BlockSubtitleEN;
        entity.BlockContentTH  = vm.BlockContentTH;
        entity.BlockContentEN  = vm.BlockContentEN;
        entity.VideoUrl        = vm.VideoUrl;
        entity.ButtonTextTH    = vm.ButtonTextTH;
        entity.ButtonTextEN    = vm.ButtonTextEN;
        entity.ButtonUrl       = vm.ButtonUrl;
        entity.LayoutType      = vm.LayoutType;
        entity.DisplayOrder    = vm.DisplayOrder;
        entity.IsActive        = vm.IsActive;
        entity.UpdatedAt       = DateTime.UtcNow;
        entity.UpdatedBy       = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Detail block updated.";
        return RedirectToAction(nameof(Index), new { activityId = entity.ActivityId });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ActivityDetailBlocks.FindAsync(id);
        if (entity is not null)
        {
            var activityId   = entity.ActivityId;
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Detail block deleted.";
            return RedirectToAction(nameof(Index), new { activityId });
        }
        return RedirectToAction(nameof(Index), new { activityId = 0 });
    }
}
