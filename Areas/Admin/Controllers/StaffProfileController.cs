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
public class StaffProfileController : Controller
{
    private static readonly string[] AllowedPdfExtensions = [".pdf"];
    private const long MaxPdfBytes = 20 * 1024 * 1024; // 20 MB

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private string CurrentUser => User.Identity?.Name ?? "system";

    public StaffProfileController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? keyword)
    {
        ViewData["Title"] = "Staff Profiles";
        ViewBag.Keyword   = keyword;

        var query = _db.StaffProfiles.AsNoTracking()
            .Where(s => !s.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(s =>
                (s.FullNameTH    != null && s.FullNameTH.Contains(keyword)) ||
                (s.FullNameEN    != null && s.FullNameEN.Contains(keyword)) ||
                (s.EmployeeCode  != null && s.EmployeeCode.Contains(keyword)) ||
                (s.Department    != null && s.Department.Contains(keyword)));

        var list = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenByDescending(s => s.IsActive)
            .ThenBy(s => s.FullNameEN)
            .ToListAsync();

        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add Staff Profile";
        return View(new StaffProfileViewModel
        {
            IsActive        = true,
            ShowPublic      = false,
            ShowContactInfo = true,
            ShowDetailPage  = true,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffProfileViewModel vm)
    {
        ViewData["Title"] = "Add Staff Profile";

        // Employee code uniqueness
        if (!string.IsNullOrWhiteSpace(vm.EmployeeCode) &&
            await _db.StaffProfiles.AnyAsync(s => s.EmployeeCode == vm.EmployeeCode && !s.IsDelete))
            ModelState.AddModelError(nameof(vm.EmployeeCode), $"Employee code '{vm.EmployeeCode}' already exists.");

        // Generate / normalise slug
        var slug = NormaliseSlug(vm.Slug, vm.FullNameEN);
        if (await _db.StaffProfiles.AnyAsync(s => s.Slug == slug && !s.IsDelete))
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..6]}";

        if (!ModelState.IsValid) return View(vm);

        // Photo upload
        string? imagePath = null;
        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/staffs");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            imagePath = path;
        }

        // PDF upload
        string? pdfPath = null;
        string? pdfName = null;
        if (vm.PdfFile is { Length: > 0 })
        {
            var (ok, err, path, name) = await SavePdfAsync(vm.PdfFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.PdfFile), err); return View(vm); }
            pdfPath = path;
            pdfName = name;
        }

        _db.StaffProfiles.Add(new StaffProfile
        {
            EmployeeCode    = vm.EmployeeCode,
            FullNameEN      = vm.FullNameEN,
            FullNameTH      = vm.FullNameTH,
            PositionEN      = vm.PositionEN,
            PositionTH      = vm.PositionTH,
            Slug            = slug,
            Department      = vm.Department,
            StaffType       = vm.StaffType,
            IsExpert        = vm.IsExpert,
            ShowPublic      = vm.ShowPublic,
            DisplayOrder    = vm.DisplayOrder,
            IsActive        = vm.IsActive,
            ProfileImageUrl = imagePath,
            MobilePhone     = vm.MobilePhone?.Trim(),
            Email           = vm.Email?.Trim(),
            BiographyTH     = vm.BiographyTH?.Trim(),
            BiographyEN     = vm.BiographyEN?.Trim(),
            AchievementTH   = vm.AchievementTH?.Trim(),
            AchievementEN   = vm.AchievementEN?.Trim(),
            PdfFileUrl      = pdfPath,
            PdfFileName     = pdfName,
            ShowContactInfo = vm.ShowContactInfo,
            ShowDetailPage  = vm.ShowDetailPage,
            StartDate       = vm.StartDate.HasValue ? DateTime.SpecifyKind(vm.StartDate.Value, DateTimeKind.Utc) : null,
            EndDate         = vm.EndDate.HasValue   ? DateTime.SpecifyKind(vm.EndDate.Value,   DateTimeKind.Utc) : null,
            CreatedAt       = DateTime.UtcNow,
            CreatedBy       = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Staff profile added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Staff Profile";
        var s = await _db.StaffProfiles.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (s is null) return NotFound();

        return View(new StaffProfileViewModel
        {
            Id               = s.Id,
            EmployeeCode     = s.EmployeeCode,
            FullNameEN       = s.FullNameEN,
            FullNameTH       = s.FullNameTH,
            PositionEN       = s.PositionEN,
            PositionTH       = s.PositionTH,
            Slug             = s.Slug,
            Department       = s.Department,
            StaffType        = s.StaffType,
            IsExpert         = s.IsExpert,
            ShowPublic       = s.ShowPublic,
            DisplayOrder     = s.DisplayOrder,
            IsActive         = s.IsActive,
            ExistingImageUrl = s.ProfileImageUrl,
            MobilePhone      = s.MobilePhone,
            Email            = s.Email,
            BiographyTH      = s.BiographyTH,
            BiographyEN      = s.BiographyEN,
            AchievementTH    = s.AchievementTH,
            AchievementEN    = s.AchievementEN,
            ExistingPdfUrl   = s.PdfFileUrl,
            ExistingPdfName  = s.PdfFileName,
            ShowContactInfo  = s.ShowContactInfo,
            ShowDetailPage   = s.ShowDetailPage,
            StartDate        = s.StartDate,
            EndDate          = s.EndDate,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StaffProfileViewModel vm)
    {
        ViewData["Title"] = "Edit Staff Profile";

        if (!string.IsNullOrWhiteSpace(vm.EmployeeCode) &&
            await _db.StaffProfiles.AnyAsync(s => s.EmployeeCode == vm.EmployeeCode && s.Id != id && !s.IsDelete))
            ModelState.AddModelError(nameof(vm.EmployeeCode), $"Employee code '{vm.EmployeeCode}' already exists.");

        // Generate / normalise slug
        var slug = NormaliseSlug(vm.Slug, vm.FullNameEN);
        if (await _db.StaffProfiles.AnyAsync(s => s.Slug == slug && s.Id != id && !s.IsDelete))
            slug = $"{slug}-{Guid.NewGuid().ToString("N")[..6]}";

        if (!ModelState.IsValid) return View(vm);

        var entity = await _db.StaffProfiles.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        // Photo upload
        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/staffs");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            entity.ProfileImageUrl = path;
        }

        // PDF upload
        if (vm.PdfFile is { Length: > 0 })
        {
            var (ok, err, path, name) = await SavePdfAsync(vm.PdfFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.PdfFile), err); return View(vm); }
            entity.PdfFileUrl  = path;
            entity.PdfFileName = name;
        }

        entity.EmployeeCode    = vm.EmployeeCode;
        entity.FullNameEN      = vm.FullNameEN;
        entity.FullNameTH      = vm.FullNameTH;
        entity.PositionEN      = vm.PositionEN;
        entity.PositionTH      = vm.PositionTH;
        entity.Slug            = slug;
        entity.Department      = vm.Department;
        entity.StaffType       = vm.StaffType;
        entity.IsExpert        = vm.IsExpert;
        entity.ShowPublic      = vm.ShowPublic;
        entity.DisplayOrder    = vm.DisplayOrder;
        entity.IsActive        = vm.IsActive;
        entity.MobilePhone     = vm.MobilePhone?.Trim();
        entity.Email           = vm.Email?.Trim();
        entity.BiographyTH     = vm.BiographyTH?.Trim();
        entity.BiographyEN     = vm.BiographyEN?.Trim();
        entity.AchievementTH   = vm.AchievementTH?.Trim();
        entity.AchievementEN   = vm.AchievementEN?.Trim();
        entity.ShowContactInfo = vm.ShowContactInfo;
        entity.ShowDetailPage  = vm.ShowDetailPage;
        entity.StartDate       = vm.StartDate.HasValue ? DateTime.SpecifyKind(vm.StartDate.Value, DateTimeKind.Utc) : null;
        entity.EndDate         = vm.EndDate.HasValue   ? DateTime.SpecifyKind(vm.EndDate.Value,   DateTimeKind.Utc) : null;
        entity.UpdatedAt       = DateTime.UtcNow;
        entity.UpdatedBy       = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Staff profile updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.StaffProfiles.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Staff profile deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ────────────────────────────────────────────────────────────

    private static string NormaliseSlug(string? slug, string? fallback)
    {
        if (!string.IsNullOrWhiteSpace(slug))
            return SlugHelper.Generate(slug);
        return SlugHelper.Generate(fallback);
    }

    private async Task<(bool ok, string error, string path, string name)> SavePdfAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedPdfExtensions.Contains(ext))
            return (false, "Only PDF files are allowed.", string.Empty, string.Empty);

        if (file.Length > MaxPdfBytes)
            return (false, "PDF size must not exceed 20 MB.", string.Empty, string.Empty);

        var dir = Path.Combine(_env.WebRootPath, "uploads", "staff-profiles", "pdfs");
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}.pdf";
        var filePath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var originalName = Path.GetFileNameWithoutExtension(file.FileName);
        return (true, string.Empty, $"/uploads/staff-profiles/pdfs/{fileName}", originalName);
    }
}
