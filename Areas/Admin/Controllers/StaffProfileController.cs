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
                (s.FullNameTH != null && s.FullNameTH.Contains(keyword)) ||
                (s.FullNameEN != null && s.FullNameEN.Contains(keyword)) ||
                (s.EmployeeCode != null && s.EmployeeCode.Contains(keyword)) ||
                (s.Department != null && s.Department.Contains(keyword)));

        var list = await query
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.FullNameEN)
            .ToListAsync();

        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        ViewData["Title"] = "Add Staff Profile";
        return View(new StaffProfileViewModel { IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StaffProfileViewModel vm)
    {
        ViewData["Title"] = "Add Staff Profile";

        if (!string.IsNullOrWhiteSpace(vm.EmployeeCode) &&
            await _db.StaffProfiles.AnyAsync(s => s.EmployeeCode == vm.EmployeeCode && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.EmployeeCode), $"Employee code '{vm.EmployeeCode}' already exists.");
        }

        if (!ModelState.IsValid) return View(vm);

        string? imagePath = null;
        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/staffs");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            imagePath = path;
        }

        _db.StaffProfiles.Add(new StaffProfile
        {
            EmployeeCode    = vm.EmployeeCode,
            FullNameTH      = vm.FullNameTH,
            FullNameEN      = vm.FullNameEN,
            PositionTH      = vm.PositionTH,
            PositionEN      = vm.PositionEN,
            Department      = vm.Department,
            StaffType       = vm.StaffType,
            IsExpert        = vm.IsExpert,
            ShowPublic      = vm.ShowPublic,
            ProfileImageUrl = imagePath,
            StartDate       = vm.StartDate.HasValue ? DateTime.SpecifyKind(vm.StartDate.Value, DateTimeKind.Utc) : null,
            EndDate         = vm.EndDate.HasValue   ? DateTime.SpecifyKind(vm.EndDate.Value,   DateTimeKind.Utc) : null,
            IsActive        = vm.IsActive,
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
            FullNameTH       = s.FullNameTH,
            FullNameEN       = s.FullNameEN,
            PositionTH       = s.PositionTH,
            PositionEN       = s.PositionEN,
            Department       = s.Department,
            StaffType        = s.StaffType,
            IsExpert         = s.IsExpert,
            ShowPublic       = s.ShowPublic,
            ExistingImageUrl = s.ProfileImageUrl,
            StartDate        = s.StartDate,
            EndDate          = s.EndDate,
            IsActive         = s.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StaffProfileViewModel vm)
    {
        ViewData["Title"] = "Edit Staff Profile";

        if (!string.IsNullOrWhiteSpace(vm.EmployeeCode) &&
            await _db.StaffProfiles.AnyAsync(s => s.EmployeeCode == vm.EmployeeCode && s.Id != id && !s.IsDelete))
        {
            ModelState.AddModelError(nameof(vm.EmployeeCode), $"Employee code '{vm.EmployeeCode}' already exists.");
        }

        if (!ModelState.IsValid) return View(vm);

        var entity = await _db.StaffProfiles.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await ImageUploadHelper.SaveAsync(vm.ImageFile, _env.WebRootPath, "uploads/about/staffs");
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), err); return View(vm); }
            entity.ProfileImageUrl = path;
        }

        entity.EmployeeCode = vm.EmployeeCode;
        entity.FullNameTH   = vm.FullNameTH;
        entity.FullNameEN   = vm.FullNameEN;
        entity.PositionTH   = vm.PositionTH;
        entity.PositionEN   = vm.PositionEN;
        entity.Department   = vm.Department;
        entity.StaffType    = vm.StaffType;
        entity.IsExpert     = vm.IsExpert;
        entity.ShowPublic   = vm.ShowPublic;
        entity.StartDate    = vm.StartDate.HasValue ? DateTime.SpecifyKind(vm.StartDate.Value, DateTimeKind.Utc) : null;
        entity.EndDate      = vm.EndDate.HasValue   ? DateTime.SpecifyKind(vm.EndDate.Value,   DateTimeKind.Utc) : null;
        entity.IsActive     = vm.IsActive;
        entity.UpdatedAt    = DateTime.UtcNow;
        entity.UpdatedBy    = CurrentUser;

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
}
