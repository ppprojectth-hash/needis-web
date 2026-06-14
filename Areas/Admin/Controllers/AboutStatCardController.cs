using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("About.View")]
public class AboutStatCardController : Controller
{
    private static readonly string[] ValidSourceTypes =
        ["Manual", "GlobalBrandCount", "ExpertStaffCount", "ProductSoldCount"];

    private readonly AppDbContext _db;
    private string CurrentUser => User.Identity?.Name ?? "system";

    public AboutStatCardController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ViewData["Title"] = "Stat Cards";

        var cards = await _db.AboutStatCards.AsNoTracking()
            .Where(c => !c.IsDelete)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        // Pre-compute values for all dynamic source types
        ViewBag.BrandCount    = await _db.BrandPartners.CountAsync(b => b.IsActive && !b.IsDelete && b.IsGlobalBrand);
        ViewBag.ExpertCount   = await _db.StaffProfiles.CountAsync(s => s.IsActive && !s.IsDelete && s.IsExpert);
        ViewBag.ProductSoldSum = await _db.ProductSales
            .Where(ps => ps.IsActive && !ps.IsDelete && ps.CountInAboutStats)
            .SumAsync(ps => (int?)ps.Quantity) ?? 0;

        return View(cards);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"]          = "Add Stat Card";
        ViewBag.ValidSourceTypes   = ValidSourceTypes;
        return View(new AboutStatCardViewModel { SourceType = "Manual", IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AboutStatCardViewModel vm)
    {
        ViewData["Title"]        = "Add Stat Card";
        ViewBag.ValidSourceTypes = ValidSourceTypes;

        if (!ValidSourceTypes.Contains(vm.SourceType))
            ModelState.AddModelError(nameof(vm.SourceType), "Invalid source type.");

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.AboutStatCards.AnyAsync(c => c.StatKey == vm.StatKey && !c.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.StatKey), $"StatKey '{vm.StatKey}' already exists.");
            return View(vm);
        }

        _db.AboutStatCards.Add(new AboutStatCard
        {
            StatKey       = vm.StatKey,
            LabelTH       = vm.LabelTH,
            LabelEN       = vm.LabelEN,
            DescriptionTH = vm.DescriptionTH,
            DescriptionEN = vm.DescriptionEN,
            SourceType    = vm.SourceType,
            ManualValue   = vm.ManualValue,
            Prefix        = vm.Prefix,
            Suffix        = vm.Suffix,
            IconUrl       = vm.IconUrl,
            CardStyle     = vm.CardStyle,
            DisplayOrder  = vm.DisplayOrder,
            IsActive      = vm.IsActive,
            CreatedAt     = DateTime.UtcNow,
            CreatedBy     = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Stat card created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"]        = "Edit Stat Card";
        ViewBag.ValidSourceTypes = ValidSourceTypes;

        var c = await _db.AboutStatCards.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (c is null) return NotFound();

        return View(new AboutStatCardViewModel
        {
            Id            = c.Id,
            StatKey       = c.StatKey,
            LabelTH       = c.LabelTH,
            LabelEN       = c.LabelEN,
            DescriptionTH = c.DescriptionTH,
            DescriptionEN = c.DescriptionEN,
            SourceType    = c.SourceType,
            ManualValue   = c.ManualValue,
            Prefix        = c.Prefix,
            Suffix        = c.Suffix,
            IconUrl       = c.IconUrl,
            CardStyle     = c.CardStyle,
            DisplayOrder  = c.DisplayOrder,
            IsActive      = c.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AboutStatCardViewModel vm)
    {
        ViewData["Title"]        = "Edit Stat Card";
        ViewBag.ValidSourceTypes = ValidSourceTypes;

        if (!ValidSourceTypes.Contains(vm.SourceType))
            ModelState.AddModelError(nameof(vm.SourceType), "Invalid source type.");

        if (!ModelState.IsValid)
            return View(vm);

        if (await _db.AboutStatCards.AnyAsync(c => c.StatKey == vm.StatKey && c.Id != id && !c.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.StatKey), $"StatKey '{vm.StatKey}' already exists.");
            return View(vm);
        }

        var entity = await _db.AboutStatCards.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        entity.StatKey       = vm.StatKey;
        entity.LabelTH       = vm.LabelTH;
        entity.LabelEN       = vm.LabelEN;
        entity.DescriptionTH = vm.DescriptionTH;
        entity.DescriptionEN = vm.DescriptionEN;
        entity.SourceType    = vm.SourceType;
        entity.ManualValue   = vm.ManualValue;
        entity.Prefix        = vm.Prefix;
        entity.Suffix        = vm.Suffix;
        entity.IconUrl       = vm.IconUrl;
        entity.CardStyle     = vm.CardStyle;
        entity.DisplayOrder  = vm.DisplayOrder;
        entity.IsActive      = vm.IsActive;
        entity.UpdatedAt     = DateTime.UtcNow;
        entity.UpdatedBy     = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Stat card updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.AboutStatCards.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Stat card deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
