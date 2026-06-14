using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Activity.Edit")]
public class ActivityRelatedItemController : Controller
{
    private readonly AppDbContext _db;

    public ActivityRelatedItemController(AppDbContext db)
    {
        _db = db;
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
        ViewData["Title"] = $"Related Items — {activity.ActivityTitleEN}";

        var list = await _db.ActivityRelatedItems.AsNoTracking()
            .Where(r => r.ActivityId == activityId)
            .Include(r => r.RelatedActivity)
            .OrderBy(r => r.DisplayOrder)
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

        ViewBag.Activity   = activity;
        ViewData["Title"]  = "Add Related Activity";
        ViewBag.Activities = await GetEligibleActivities(activityId);

        return View(new ActivityRelatedItemViewModel { ActivityId = activityId, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ActivityRelatedItemViewModel vm)
    {
        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ActivityId && !x.IsDelete);
        ViewBag.Activity   = activity;
        ViewData["Title"]  = "Add Related Activity";
        ViewBag.Activities = await GetEligibleActivities(vm.ActivityId);

        if (vm.RelatedActivityId == vm.ActivityId)
        {
            ModelState.AddModelError(nameof(vm.RelatedActivityId), "Cannot relate an activity to itself.");
        }

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.ActivityRelatedItems.AnyAsync(r =>
                r.ActivityId == vm.ActivityId && r.RelatedActivityId == vm.RelatedActivityId))
        {
            ModelState.AddModelError(nameof(vm.RelatedActivityId), "This activity is already related.");
            return View(vm);
        }

        _db.ActivityRelatedItems.Add(new ActivityRelatedItem
        {
            ActivityId        = vm.ActivityId,
            RelatedActivityId = vm.RelatedActivityId,
            DisplayOrder      = vm.DisplayOrder,
            IsActive          = vm.IsActive,
            CreatedAt         = DateTime.UtcNow,
            CreatedBy         = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Related activity added.";
        return RedirectToAction(nameof(Index), new { activityId = vm.ActivityId });
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Related Activity";

        var item = await _db.ActivityRelatedItems.AsNoTracking()
            .Include(r => r.RelatedActivity)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (item is null) return NotFound();

        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == item.ActivityId && !x.IsDelete);
        ViewBag.Activity  = activity;
        ViewBag.Activities = await GetEligibleActivities(item.ActivityId);

        return View(new ActivityRelatedItemViewModel
        {
            Id                = item.Id,
            ActivityId        = item.ActivityId,
            RelatedActivityId = item.RelatedActivityId,
            DisplayOrder      = item.DisplayOrder,
            IsActive          = item.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActivityRelatedItemViewModel vm)
    {
        ViewData["Title"]  = "Edit Related Activity";
        ViewBag.Activities = await GetEligibleActivities(vm.ActivityId);

        var activity = await _db.Activities.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ActivityId && !x.IsDelete);
        ViewBag.Activity = activity;

        if (vm.RelatedActivityId == vm.ActivityId)
            ModelState.AddModelError(nameof(vm.RelatedActivityId), "Cannot relate an activity to itself.");

        if (!ModelState.IsValid)
            return View(vm);

        var entity = await _db.ActivityRelatedItems.FindAsync(id);
        if (entity is null) return NotFound();

        if (await _db.ActivityRelatedItems.AnyAsync(r =>
                r.ActivityId == vm.ActivityId && r.RelatedActivityId == vm.RelatedActivityId && r.Id != id))
        {
            ModelState.AddModelError(nameof(vm.RelatedActivityId), "This activity is already related.");
            return View(vm);
        }

        entity.RelatedActivityId = vm.RelatedActivityId;
        entity.DisplayOrder      = vm.DisplayOrder;
        entity.IsActive          = vm.IsActive;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Related activity updated.";
        return RedirectToAction(nameof(Index), new { activityId = entity.ActivityId });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ActivityRelatedItems.FindAsync(id);
        if (entity is not null)
        {
            var activityId = entity.ActivityId;
            _db.ActivityRelatedItems.Remove(entity);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Related activity removed.";
            return RedirectToAction(nameof(Index), new { activityId });
        }
        return RedirectToAction(nameof(Index), new { activityId = 0 });
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private async Task<List<Needis.Web.Models.Activity>> GetEligibleActivities(int excludeId)
    {
        return await _db.Activities.AsNoTracking()
            .Where(a => !a.IsDelete && a.IsActive && a.IsPublished && a.Id != excludeId)
            .OrderBy(a => a.ActivityTitleEN)
            .ToListAsync();
    }
}
