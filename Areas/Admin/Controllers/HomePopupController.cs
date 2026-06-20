using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class HomePopupController : Controller
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB

    private readonly AppDbContext        _db;
    private readonly IWebHostEnvironment _env;

    public HomePopupController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    // ── Index ───────────────────────────────────────────────────────────────

    [RequirePermission("HomePopup.View")]
    public async Task<IActionResult> Index()
    {
        var popups = await _db.HomePopups
            .AsNoTracking()
            .Where(p => !p.IsDelete)
            .OrderBy(p => p.DisplayOrder)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(popups);
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("HomePopup.Create")]
    public IActionResult Create() => View(new HomePopupViewModel { IsActive = true, ShowOnlyHomePage = true, ShowOncePerDay = true, OpenLinkInNewTab = true, DisplayDelaySeconds = 1 });

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("HomePopup.Create")]
    public async Task<IActionResult> Create(HomePopupViewModel vm)
    {
        if (vm.ImageFile is not { Length: > 0 })
            ModelState.AddModelError(nameof(vm.ImageFile), "Popup image is required.");

        if (!ModelState.IsValid) return View(vm);

        var popup = new HomePopup
        {
            TitleTH              = vm.TitleTH             ?? string.Empty,
            TitleEN              = vm.TitleEN              ?? string.Empty,
            DescriptionTH        = vm.DescriptionTH,
            DescriptionEN        = vm.DescriptionEN,
            LinkUrl              = vm.LinkUrl,
            ButtonTextTH         = vm.ButtonTextTH,
            ButtonTextEN         = vm.ButtonTextEN,
            PopupType            = vm.PopupType,
            DisplayOrder         = vm.DisplayOrder,
            IsActive             = vm.IsActive,
            ShowOnlyHomePage     = vm.ShowOnlyHomePage,
            ShowOncePerSession   = vm.ShowOncePerSession,
            ShowOncePerDay       = vm.ShowOncePerDay,
            OpenLinkInNewTab     = vm.OpenLinkInNewTab,
            DisplayDelaySeconds  = vm.DisplayDelaySeconds,
            StartDateUtc         = ToUtc(vm.StartDate),
            EndDateUtc           = ToUtc(vm.EndDate),
            CreatedAt            = DateTime.UtcNow,
            CreatedBy            = User.Identity?.Name,
        };

        var (ok, err, path) = await SaveImageAsync(vm.ImageFile!);
        if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
        popup.ImageUrl = path;

        if (vm.MobileImageFile is { Length: > 0 })
        {
            var (mok, merr, mpath) = await SaveImageAsync(vm.MobileImageFile, "mobile");
            if (!mok) { ModelState.AddModelError(nameof(vm.MobileImageFile), merr); return View(vm); }
            popup.MobileImageUrl = mpath;
        }

        _db.HomePopups.Add(popup);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Popup created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("HomePopup.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var popup = await _db.HomePopups.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id && !p.IsDelete);
        if (popup is null) return NotFound();
        return View(MapToViewModel(popup));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("HomePopup.Edit")]
    public async Task<IActionResult> Edit(int id, HomePopupViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var popup = await _db.HomePopups.FindAsync(id);
        if (popup is null || popup.IsDelete) return NotFound();

        popup.TitleTH             = vm.TitleTH            ?? string.Empty;
        popup.TitleEN             = vm.TitleEN             ?? string.Empty;
        popup.DescriptionTH       = vm.DescriptionTH;
        popup.DescriptionEN       = vm.DescriptionEN;
        popup.LinkUrl             = vm.LinkUrl;
        popup.ButtonTextTH        = vm.ButtonTextTH;
        popup.ButtonTextEN        = vm.ButtonTextEN;
        popup.PopupType           = vm.PopupType;
        popup.DisplayOrder        = vm.DisplayOrder;
        popup.IsActive            = vm.IsActive;
        popup.ShowOnlyHomePage    = vm.ShowOnlyHomePage;
        popup.ShowOncePerSession  = vm.ShowOncePerSession;
        popup.ShowOncePerDay      = vm.ShowOncePerDay;
        popup.OpenLinkInNewTab    = vm.OpenLinkInNewTab;
        popup.DisplayDelaySeconds = vm.DisplayDelaySeconds;
        popup.StartDateUtc        = ToUtc(vm.StartDate);
        popup.EndDateUtc          = ToUtc(vm.EndDate);
        popup.UpdatedAt           = DateTime.UtcNow;
        popup.UpdatedBy           = User.Identity?.Name;

        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await SaveImageAsync(vm.ImageFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            popup.ImageUrl = path;
        }

        if (vm.MobileImageFile is { Length: > 0 })
        {
            var (mok, merr, mpath) = await SaveImageAsync(vm.MobileImageFile, "mobile");
            if (!mok) { ModelState.AddModelError(nameof(vm.MobileImageFile), merr); return View(vm); }
            popup.MobileImageUrl = mpath;
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Popup updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Toggle Active ────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("HomePopup.Edit")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var popup = await _db.HomePopups.FindAsync(id);
        if (popup is not null && !popup.IsDelete)
        {
            popup.IsActive  = !popup.IsActive;
            popup.UpdatedAt = DateTime.UtcNow;
            popup.UpdatedBy = User.Identity?.Name;
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Popup status updated.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("HomePopup.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var popup = await _db.HomePopups.FindAsync(id);
        if (popup is not null)
        {
            popup.IsDelete  = true;
            popup.IsActive  = false;
            popup.UpdatedAt = DateTime.UtcNow;
            popup.UpdatedBy = User.Identity?.Name;
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Popup deleted.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private async Task<(bool ok, string error, string path)> SaveImageAsync(IFormFile file, string subFolder = "")
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext))
            return (false, $"File type '{ext}' not allowed. Accepted: jpg, jpeg, png, webp, gif.", "");
        if (file.Length > MaxImageSize)
            return (false, "Image must not exceed 5 MB.", "");

        var dir = string.IsNullOrEmpty(subFolder)
            ? Path.Combine(_env.WebRootPath, "uploads", "home-popups")
            : Path.Combine(_env.WebRootPath, "uploads", "home-popups", subFolder);
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        await using var stream = new FileStream(Path.Combine(dir, fileName), FileMode.Create);
        await file.CopyToAsync(stream);

        var relativePath = string.IsNullOrEmpty(subFolder)
            ? $"/uploads/home-popups/{fileName}"
            : $"/uploads/home-popups/{subFolder}/{fileName}";

        return (true, "", relativePath);
    }

    private static HomePopupViewModel MapToViewModel(HomePopup p) => new()
    {
        Id                  = p.Id,
        TitleTH             = p.TitleTH,
        TitleEN             = p.TitleEN,
        DescriptionTH       = p.DescriptionTH,
        DescriptionEN       = p.DescriptionEN,
        ImageUrl            = p.ImageUrl,
        MobileImageUrl      = p.MobileImageUrl,
        LinkUrl             = p.LinkUrl,
        ButtonTextTH        = p.ButtonTextTH,
        ButtonTextEN        = p.ButtonTextEN,
        PopupType           = p.PopupType,
        DisplayOrder        = p.DisplayOrder,
        IsActive            = p.IsActive,
        ShowOnlyHomePage    = p.ShowOnlyHomePage,
        ShowOncePerSession  = p.ShowOncePerSession,
        ShowOncePerDay      = p.ShowOncePerDay,
        OpenLinkInNewTab    = p.OpenLinkInNewTab,
        DisplayDelaySeconds = p.DisplayDelaySeconds,
        StartDate           = p.StartDateUtc,
        EndDate             = p.EndDateUtc,
    };

    private static DateTime? ToUtc(DateTime? dt) =>
        dt.HasValue ? DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc) : null;
}
