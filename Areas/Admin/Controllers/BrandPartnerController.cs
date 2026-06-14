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
public class BrandPartnerController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private string CurrentUser => User.Identity?.Name ?? "system";

    public BrandPartnerController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewData["Title"] = "Brand Partners";
        ViewBag.Keyword   = keyword;

        var query = _db.BrandPartners.AsNoTracking()
            .Where(b => !b.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(b => b.BrandName.Contains(keyword));

        var list = await query
            .OrderBy(b => b.DisplayOrder)
            .ThenBy(b => b.BrandName)
            .ToListAsync();

        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add Brand Partner";
        return View(new BrandPartnerViewModel { ShowOnPartnerSection = true, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BrandPartnerViewModel vm)
    {
        ViewData["Title"] = "Add Brand Partner";
        if (!ModelState.IsValid) return View(vm);

        string? logoPath = null;
        if (vm.LogoFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.LogoFile, _env.WebRootPath, "uploads/about/brands");
            if (!ok) { ModelState.AddModelError(nameof(vm.LogoFile), err); return View(vm); }
            logoPath = path;
        }

        _db.BrandPartners.Add(new BrandPartner
        {
            BrandName            = vm.BrandName,
            LogoUrl              = logoPath,
            WebsiteUrl           = vm.WebsiteUrl,
            BrandType            = vm.BrandType,
            IsGlobalBrand        = vm.IsGlobalBrand,
            ShowOnPartnerSection = vm.ShowOnPartnerSection,
            DisplayOrder         = vm.DisplayOrder,
            IsActive             = vm.IsActive,
            CreatedAt            = DateTime.UtcNow,
            CreatedBy            = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Brand partner added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Brand Partner";
        var b = await _db.BrandPartners.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (b is null) return NotFound();

        return View(new BrandPartnerViewModel
        {
            Id                   = b.Id,
            BrandName            = b.BrandName,
            ExistingLogoUrl      = b.LogoUrl,
            WebsiteUrl           = b.WebsiteUrl,
            BrandType            = b.BrandType,
            IsGlobalBrand        = b.IsGlobalBrand,
            ShowOnPartnerSection = b.ShowOnPartnerSection,
            DisplayOrder         = b.DisplayOrder,
            IsActive             = b.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BrandPartnerViewModel vm)
    {
        ViewData["Title"] = "Edit Brand Partner";
        if (!ModelState.IsValid) return View(vm);

        var entity = await _db.BrandPartners.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.LogoFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.LogoFile, _env.WebRootPath, "uploads/about/brands");
            if (!ok) { ModelState.AddModelError(nameof(vm.LogoFile), err); return View(vm); }
            entity.LogoUrl = path;
        }

        entity.BrandName            = vm.BrandName;
        entity.WebsiteUrl           = vm.WebsiteUrl;
        entity.BrandType            = vm.BrandType;
        entity.IsGlobalBrand        = vm.IsGlobalBrand;
        entity.ShowOnPartnerSection = vm.ShowOnPartnerSection;
        entity.DisplayOrder         = vm.DisplayOrder;
        entity.IsActive             = vm.IsActive;
        entity.UpdatedAt            = DateTime.UtcNow;
        entity.UpdatedBy            = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Brand partner updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.BrandPartners.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Brand partner deleted.";
        }
        return RedirectToAction(nameof(Index));
    }
}
