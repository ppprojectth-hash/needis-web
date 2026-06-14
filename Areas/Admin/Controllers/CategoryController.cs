using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class CategoryController : Controller
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".svg"];
    private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public CategoryController(AppDbContext db, IWebHostEnvironment env)
    {
        _db  = db;
        _env = env;
    }

    // ── Index ───────────────────────────────────────────────────────────────

    [RequirePermission("Category.View")]
    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.ProductCategories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(c =>
                c.NameEN.Contains(q) ||
                c.NameTH.Contains(q) ||
                c.Slug.Contains(q));

        var items = await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEN)
            .Select(c => new CategoryListItem
            {
                Id           = c.Id,
                NameEN       = c.NameEN,
                NameTH       = c.NameTH,
                Slug         = c.Slug,
                ImagePath    = c.ImagePath,
                DisplayOrder = c.DisplayOrder,
                IsActive     = c.IsActive,
                ProductCount = c.Products.Count(),
            })
            .ToListAsync();

        ViewBag.SearchKeyword = q;
        return View(items);
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Category.Create")]
    public IActionResult Create() => View(new ProductCategoryViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Category.Create")]
    public async Task<IActionResult> Create(ProductCategoryViewModel vm)
    {
        vm.Slug = NormalizeSlug(vm.Slug ?? "");
        if (string.IsNullOrEmpty(vm.Slug))
            ModelState.AddModelError(nameof(vm.Slug), "Slug cannot be empty. Use letters, numbers, or hyphens.");

        if (!ModelState.IsValid)
            return View(vm);

        // Duplicate slug check
        if (await _db.ProductCategories.AnyAsync(c => c.Slug == vm.Slug))
        {
            ModelState.AddModelError(nameof(vm.Slug), $"Slug '{vm.Slug}' is already in use.");
            return View(vm);
        }

        string? imagePath = null;
        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, error, path) = await SaveUploadAsync(vm.ImageFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), error); return View(vm); }
            imagePath = path;
        }

        var category = new ProductCategory
        {
            NameTH             = vm.CategoryNameTH   ?? string.Empty,
            NameEN             = vm.CategoryNameEN    ?? string.Empty,
            Slug               = vm.Slug,
            ShortDescriptionTH = vm.ShortDescriptionTH,
            ShortDescriptionEN = vm.ShortDescriptionEN,
            ImagePath          = imagePath,
            DisplayOrder       = vm.DisplayOrder,
            IsActive           = vm.IsActive,
            CreatedAt          = DateTime.UtcNow,
            UpdatedAt          = DateTime.UtcNow,
        };

        _db.ProductCategories.Add(category);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Category created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Category.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _db.ProductCategories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return NotFound();

        return View(MapToViewModel(category));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Category.Edit")]
    public async Task<IActionResult> Edit(int id, ProductCategoryViewModel vm)
    {
        vm.Slug = NormalizeSlug(vm.Slug ?? "");
        if (string.IsNullOrEmpty(vm.Slug))
            ModelState.AddModelError(nameof(vm.Slug), "Slug cannot be empty. Use letters, numbers, or hyphens.");

        if (!ModelState.IsValid)
            return View(vm);

        // Duplicate slug check (exclude self)
        if (await _db.ProductCategories.AnyAsync(c => c.Slug == vm.Slug && c.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Slug), $"Slug '{vm.Slug}' is already in use by another category.");
            return View(vm);
        }

        var category = await _db.ProductCategories.FindAsync(id);
        if (category is null) return NotFound();

        if (vm.ImageFile is { Length: > 0 })
        {
            var (ok, error, path) = await SaveUploadAsync(vm.ImageFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.ImageFile), error); return View(vm); }
            category.ImagePath = path;
        }

        category.NameTH             = vm.CategoryNameTH   ?? string.Empty;
        category.NameEN             = vm.CategoryNameEN    ?? string.Empty;
        category.Slug               = vm.Slug;
        category.ShortDescriptionTH = vm.ShortDescriptionTH;
        category.ShortDescriptionEN = vm.ShortDescriptionEN;
        category.DisplayOrder       = vm.DisplayOrder;
        category.IsActive           = vm.IsActive;
        category.UpdatedAt          = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Category updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Category.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _db.ProductCategories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            TempData["Success"] = "Category not found.";
            return RedirectToAction(nameof(Index));
        }

        if (category.Products.Count != 0)
        {
            category.IsActive  = false;
            category.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Warning"] = "Category has products, so it was deactivated instead of deleted.";
        }
        else
        {
            _db.ProductCategories.Remove(category);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Category deleted successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Normalizes input to a URL-safe slug:
    /// lowercase, spaces → hyphens, strips non-alphanumeric/hyphen chars,
    /// collapses consecutive hyphens, trims leading/trailing hyphens.
    /// </summary>
    private static string NormalizeSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var slug = input.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"-{2,}", "-");
        return slug.Trim('-');
    }

    private async Task<(bool ok, string error, string path)> SaveUploadAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
            return (false,
                $"File type '{ext}' is not allowed. Accepted: jpg, jpeg, png, webp, svg.",
                string.Empty);

        if (file.Length > MaxFileSize)
            return (false, "File size must not exceed 2 MB.", string.Empty);

        var dir = Path.Combine(_env.WebRootPath, "uploads", "categories");
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (true, string.Empty, $"/uploads/categories/{fileName}");
    }

    private static ProductCategoryViewModel MapToViewModel(ProductCategory c) => new()
    {
        ProductCategoryId  = c.Id,
        CategoryNameTH     = c.NameTH,
        CategoryNameEN     = c.NameEN,
        Slug               = c.Slug,
        ShortDescriptionTH = c.ShortDescriptionTH,
        ShortDescriptionEN = c.ShortDescriptionEN,
        ImageUrl           = c.ImagePath,
        DisplayOrder       = c.DisplayOrder,
        IsActive           = c.IsActive,
    };
}
