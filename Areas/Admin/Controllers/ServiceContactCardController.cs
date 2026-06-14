using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Service.Edit")]
public class ServiceContactCardController : Controller
{
    private readonly AppDbContext _db;

    public ServiceContactCardController(AppDbContext db)
    {
        _db = db;
    }

    private string CurrentUser => User.Identity?.Name ?? "system";

    // ── Index ────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(int serviceId)
    {
        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == serviceId && !x.IsDelete);
        if (svc is null) return NotFound();

        ViewData["Title"] = $"Contact Cards — {svc.ServiceNameEN ?? svc.ServiceCode}";
        ViewBag.Service   = svc;

        var cards = await _db.ServiceContactCards.AsNoTracking()
            .Where(c => c.ServiceId == serviceId && !c.IsDelete)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        return View(cards);
    }

    // ── Create ───────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Create(int serviceId)
    {
        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == serviceId && !x.IsDelete);
        if (svc is null) return NotFound();

        ViewData["Title"] = "Add Contact Card";
        ViewBag.Service   = svc;
        return View(new ServiceContactCardViewModel { ServiceId = serviceId, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceContactCardViewModel vm)
    {
        var svc = await _db.Services.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == vm.ServiceId && !x.IsDelete);
        if (svc is null) return NotFound();

        ViewData["Title"] = "Add Contact Card";
        ViewBag.Service   = svc;

        if (!ModelState.IsValid)
            return View(vm);

        _db.ServiceContactCards.Add(new ServiceContactCard
        {
            ServiceId             = vm.ServiceId,
            TitleTH               = vm.TitleTH,
            TitleEN               = vm.TitleEN,
            DescriptionTH         = vm.DescriptionTH,
            DescriptionEN         = vm.DescriptionEN,
            PrimaryButtonTextTH   = vm.PrimaryButtonTextTH,
            PrimaryButtonTextEN   = vm.PrimaryButtonTextEN,
            PrimaryButtonUrl      = vm.PrimaryButtonUrl,
            SecondaryButtonTextTH = vm.SecondaryButtonTextTH,
            SecondaryButtonTextEN = vm.SecondaryButtonTextEN,
            SecondaryButtonUrl    = vm.SecondaryButtonUrl,
            ContactLabelTH        = vm.ContactLabelTH,
            ContactLabelEN        = vm.ContactLabelEN,
            ContactValue          = vm.ContactValue,
            ContactIconUrl        = vm.ContactIconUrl,
            DisplayOrder          = vm.DisplayOrder,
            IsActive              = vm.IsActive,
            CreatedAt             = DateTime.UtcNow,
            CreatedBy             = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Contact card added.";
        return RedirectToAction(nameof(Index), new { serviceId = vm.ServiceId });
    }

    // ── Edit ─────────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Contact Card";
        var card = await _db.ServiceContactCards.AsNoTracking()
            .Include(c => c.Service)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDelete);
        if (card is null) return NotFound();

        ViewBag.Service = card.Service;
        return View(new ServiceContactCardViewModel
        {
            Id                    = card.Id,
            ServiceId             = card.ServiceId,
            TitleTH               = card.TitleTH,
            TitleEN               = card.TitleEN,
            DescriptionTH         = card.DescriptionTH,
            DescriptionEN         = card.DescriptionEN,
            PrimaryButtonTextTH   = card.PrimaryButtonTextTH,
            PrimaryButtonTextEN   = card.PrimaryButtonTextEN,
            PrimaryButtonUrl      = card.PrimaryButtonUrl,
            SecondaryButtonTextTH = card.SecondaryButtonTextTH,
            SecondaryButtonTextEN = card.SecondaryButtonTextEN,
            SecondaryButtonUrl    = card.SecondaryButtonUrl,
            ContactLabelTH        = card.ContactLabelTH,
            ContactLabelEN        = card.ContactLabelEN,
            ContactValue          = card.ContactValue,
            ContactIconUrl        = card.ContactIconUrl,
            DisplayOrder          = card.DisplayOrder,
            IsActive              = card.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceContactCardViewModel vm)
    {
        ViewData["Title"] = "Edit Contact Card";

        if (!ModelState.IsValid)
        {
            var svcForView = await _db.Services.AsNoTracking().FirstOrDefaultAsync(x => x.Id == vm.ServiceId);
            ViewBag.Service = svcForView;
            return View(vm);
        }

        var entity = await _db.ServiceContactCards.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        entity.TitleTH               = vm.TitleTH;
        entity.TitleEN               = vm.TitleEN;
        entity.DescriptionTH         = vm.DescriptionTH;
        entity.DescriptionEN         = vm.DescriptionEN;
        entity.PrimaryButtonTextTH   = vm.PrimaryButtonTextTH;
        entity.PrimaryButtonTextEN   = vm.PrimaryButtonTextEN;
        entity.PrimaryButtonUrl      = vm.PrimaryButtonUrl;
        entity.SecondaryButtonTextTH = vm.SecondaryButtonTextTH;
        entity.SecondaryButtonTextEN = vm.SecondaryButtonTextEN;
        entity.SecondaryButtonUrl    = vm.SecondaryButtonUrl;
        entity.ContactLabelTH        = vm.ContactLabelTH;
        entity.ContactLabelEN        = vm.ContactLabelEN;
        entity.ContactValue          = vm.ContactValue;
        entity.ContactIconUrl        = vm.ContactIconUrl;
        entity.DisplayOrder          = vm.DisplayOrder;
        entity.IsActive              = vm.IsActive;
        entity.UpdatedAt             = DateTime.UtcNow;
        entity.UpdatedBy             = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Contact card updated.";
        return RedirectToAction(nameof(Index), new { serviceId = entity.ServiceId });
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ServiceContactCards.FindAsync(id);
        if (entity is not null)
        {
            var serviceId    = entity.ServiceId;
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Contact card deleted.";
            return RedirectToAction(nameof(Index), new { serviceId });
        }
        return RedirectToAction(nameof(Index), new { serviceId = 0 });
    }
}
