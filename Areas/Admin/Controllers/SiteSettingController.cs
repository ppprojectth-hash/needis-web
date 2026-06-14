using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class SiteSettingController : Controller
{
    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".webp", ".svg", ".ico"];
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public SiteSettingController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    [HttpGet]
    [RequirePermission("SiteSetting.View")]
    public async Task<IActionResult> Index()
    {
        var setting = await GetOrCreateSettingAsync();
        return View(MapToViewModel(setting));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("SiteSetting.Edit")]
    public async Task<IActionResult> Index(SiteSettingViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var setting = await _db.SiteSettings.FirstOrDefaultAsync()
                      ?? new SiteSetting();

        // Map text fields
        setting.CompanyNameTH  = vm.CompanyNameTH  ?? string.Empty;
        setting.CompanyNameEN  = vm.CompanyNameEN  ?? string.Empty;
        setting.MainColor      = vm.MainColor?.Trim() is { Length: > 0 } c ? c : "#2d4199";
        setting.ContactPhone   = vm.Phone;
        setting.ContactEmail   = vm.Email;
        setting.AddressTH      = vm.AddressTH;
        setting.AddressEN      = vm.AddressEN;
        setting.FacebookUrl    = vm.FacebookUrl;
        setting.LineUrl        = vm.LineUrl;
        setting.LinkedInUrl    = vm.LinkedInUrl;
        setting.IsActive       = vm.IsActive;
        setting.UpdatedAt      = DateTime.UtcNow;

        // Handle logo upload
        if (vm.LogoFile is { Length: > 0 })
        {
            var (ok, error, path) = await SaveUploadAsync(vm.LogoFile, "logos");
            if (!ok)
            {
                ModelState.AddModelError(nameof(vm.LogoFile), error);
                vm.LogoUrl = setting.LogoPath;
                return View(vm);
            }
            setting.LogoPath = path;
        }

        // Handle favicon upload
        if (vm.FaviconFile is { Length: > 0 })
        {
            var (ok, error, path) = await SaveUploadAsync(vm.FaviconFile, "logos");
            if (!ok)
            {
                ModelState.AddModelError(nameof(vm.FaviconFile), error);
                vm.FaviconUrl = setting.FaviconPath;
                return View(vm);
            }
            setting.FaviconPath = path;
        }

        if (setting.Id == 0)
            _db.SiteSettings.Add(setting);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Site settings saved successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private async Task<SiteSetting> GetOrCreateSettingAsync()
    {
        var setting = await _db.SiteSettings.FirstOrDefaultAsync();
        if (setting is not null)
            return setting;

        setting = new SiteSetting
        {
            CompanyNameEN   = "Needis",
            CompanyNameTH   = "Needis",
            MainColor       = "#2d4199",
            DefaultLanguage = "EN",
            IsActive        = true,
            UpdatedAt       = DateTime.UtcNow,
        };
        _db.SiteSettings.Add(setting);
        await _db.SaveChangesAsync();
        return setting;
    }

    private static SiteSettingViewModel MapToViewModel(SiteSetting s) => new()
    {
        SiteSettingId = s.Id,
        CompanyNameTH = s.CompanyNameTH,
        CompanyNameEN = s.CompanyNameEN,
        LogoUrl       = s.LogoPath,
        FaviconUrl    = s.FaviconPath,
        MainColor     = s.MainColor,
        Phone         = s.ContactPhone,
        Email         = s.ContactEmail,
        AddressTH     = s.AddressTH,
        AddressEN     = s.AddressEN,
        FacebookUrl   = s.FacebookUrl,
        LineUrl       = s.LineUrl,
        LinkedInUrl   = s.LinkedInUrl,
        IsActive      = s.IsActive,
    };

    private async Task<(bool ok, string error, string path)> SaveUploadAsync(
        IFormFile file, string subfolder)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
            return (false, $"File type '{ext}' is not allowed. Accepted: jpg, jpeg, png, webp, svg, ico.", string.Empty);

        if (file.Length > MaxFileSize)
            return (false, "File size must not exceed 2 MB.", string.Empty);

        var dir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (true, string.Empty, $"/uploads/{subfolder}/{fileName}");
    }
}
