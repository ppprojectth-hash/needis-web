using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.Models;
using Needis.Web.Services.Content;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class ProductController : Controller
{
    private static readonly string[] AllowedImageExtensions    = [".jpg", ".jpeg", ".png", ".webp"];
    private static readonly string[] AllowedBrochureExtensions = [".pdf"];
    private const long MaxImageSize    = 5  * 1024 * 1024; // 5 MB
    private const long MaxBrochureSize = 10 * 1024 * 1024; // 10 MB

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IRichContentService _richContent;

    public ProductController(AppDbContext db, IWebHostEnvironment env, IRichContentService richContent)
    {
        _db  = db;
        _env = env;
        _richContent = richContent;
    }

    // ── Index ───────────────────────────────────────────────────────────────

    [RequirePermission("Product.View")]
    public async Task<IActionResult> Index(string? q, int? categoryId)
    {
        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p =>
                p.NameEN.Contains(q) ||
                p.NameTH.Contains(q) ||
                (p.Brand != null && p.Brand.Contains(q)) ||
                (p.Model != null && p.Model.Contains(q)) ||
                (p.Sku   != null && p.Sku.Contains(q)));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var items = await query
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.NameEN)
            .Select(p => new ProductListItem
            {
                Id              = p.Id,
                NameEN          = p.NameEN,
                NameTH          = p.NameTH,
                CategoryNameEN  = p.Category.NameEN,
                Brand           = p.Brand,
                Model           = p.Model,
                Sku             = p.Sku,
                MainImagePath   = p.MainImagePath,
                Price           = p.Price,
                IsPriceVisible  = p.IsPriceVisible,
                IsFeatured      = p.IsFeatured,
                IsPromotion     = p.IsPromotion,
                IsActive        = p.IsActive,
                DisplayOrder    = p.DisplayOrder,
                HasVideo        = p.ShowYoutubeVideo && p.YoutubeVideoUrl != null && p.YoutubeVideoUrl != "",
            })
            .ToListAsync();

        var categories = await _db.ProductCategories
            .AsNoTracking()
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEN)
            .Select(c => new { c.Id, c.NameEN })
            .ToListAsync();

        ViewBag.SearchKeyword  = q;
        ViewBag.CategoryId     = categoryId;
        ViewBag.CategoryFilter = new SelectList(categories, "Id", "NameEN", categoryId);

        return View(items);
    }

    // ── Create ──────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Product.Create")]
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAsync(null);
        return View(new ProductViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Create")]
    public async Task<IActionResult> Create(ProductViewModel vm)
    {
        vm.Slug = NormalizeSlug(vm.Slug ?? "");
        if (string.IsNullOrEmpty(vm.Slug))
            ModelState.AddModelError(nameof(vm.Slug), "Slug cannot be empty. Use letters, numbers, or hyphens.");

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(vm.ProductCategoryId);
            return View(vm);
        }

        if (await _db.Products.AnyAsync(p => p.Slug == vm.Slug))
        {
            ModelState.AddModelError(nameof(vm.Slug), $"Slug '{vm.Slug}' is already in use.");
            await LoadCategoriesAsync(vm.ProductCategoryId);
            return View(vm);
        }

        string? imagePath    = null;
        string? brochurePath = null;

        if (vm.MainImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await SaveImageAsync(vm.MainImageFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.MainImageFile), err); await LoadCategoriesAsync(vm.ProductCategoryId); return View(vm); }
            imagePath = path;
        }

        if (vm.BrochureFile is { Length: > 0 })
        {
            var (ok, err, path) = await SaveBrochureAsync(vm.BrochureFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.BrochureFile), err); await LoadCategoriesAsync(vm.ProductCategoryId); return View(vm); }
            brochurePath = path;
        }

        var product = new Product
        {
            CategoryId                  = vm.ProductCategoryId,
            NameTH                      = vm.ProductNameTH     ?? string.Empty,
            NameEN                      = vm.ProductNameEN      ?? string.Empty,
            Slug                        = vm.Slug,
            Brand                       = vm.Brand,
            Model                       = vm.Model,
            Sku                         = vm.PartNumber,
            ShortDescriptionTH          = vm.ShortDescriptionTH,
            ShortDescriptionEN          = vm.ShortDescriptionEN,
            FullDescriptionTH           = SanitizedOrNull(vm.DescriptionTH),
            FullDescriptionEN           = SanitizedOrNull(vm.DescriptionEN),
            SpecificationTH             = SanitizedOrNull(vm.SpecificationTH),
            SpecificationEN             = SanitizedOrNull(vm.SpecificationEN),
            Price                       = vm.Price,
            IsPriceVisible              = vm.IsPriceVisible,
            MainImagePath               = imagePath,
            BrochureFilePath            = brochurePath,
            DisplayOrder                = vm.DisplayOrder,
            IsFeatured                  = vm.IsFeatured,
            IsPromotion                 = vm.IsPromotion,
            PromotionLabelTH            = vm.PromotionLabelTH,
            PromotionLabelEN            = vm.PromotionLabelEN,
            IsActive                    = vm.IsActive,
            ShowYoutubeVideo            = vm.ShowYoutubeVideo,
            YoutubeVideoUrl             = string.IsNullOrWhiteSpace(vm.YoutubeVideoUrl) ? null : vm.YoutubeVideoUrl.Trim(),
            YoutubeVideoTitleTH         = vm.YoutubeVideoTitleTH,
            YoutubeVideoTitleEN         = vm.YoutubeVideoTitleEN,
            YoutubeVideoDescriptionTH   = vm.YoutubeVideoDescriptionTH,
            YoutubeVideoDescriptionEN   = vm.YoutubeVideoDescriptionEN,
            CreatedAt                   = DateTime.UtcNow,
            UpdatedAt                   = DateTime.UtcNow,
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Product created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Edit ────────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _db.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) return NotFound();

        await LoadCategoriesAsync(product.CategoryId);
        return View(MapToViewModel(product));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Edit(int id, ProductViewModel vm)
    {
        vm.Slug = NormalizeSlug(vm.Slug ?? "");
        if (string.IsNullOrEmpty(vm.Slug))
            ModelState.AddModelError(nameof(vm.Slug), "Slug cannot be empty. Use letters, numbers, or hyphens.");

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAsync(vm.ProductCategoryId);
            return View(vm);
        }

        if (await _db.Products.AnyAsync(p => p.Slug == vm.Slug && p.Id != id))
        {
            ModelState.AddModelError(nameof(vm.Slug), $"Slug '{vm.Slug}' is already in use by another product.");
            await LoadCategoriesAsync(vm.ProductCategoryId);
            return View(vm);
        }

        var product = await _db.Products.FindAsync(id);
        if (product is null) return NotFound();

        if (vm.MainImageFile is { Length: > 0 })
        {
            var (ok, err, path) = await SaveImageAsync(vm.MainImageFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.MainImageFile), err); await LoadCategoriesAsync(vm.ProductCategoryId); return View(vm); }
            product.MainImagePath = path;
        }

        if (vm.BrochureFile is { Length: > 0 })
        {
            var (ok, err, path) = await SaveBrochureAsync(vm.BrochureFile);
            if (!ok) { ModelState.AddModelError(nameof(vm.BrochureFile), err); await LoadCategoriesAsync(vm.ProductCategoryId); return View(vm); }
            product.BrochureFilePath = path;
        }

        product.CategoryId                  = vm.ProductCategoryId;
        product.NameTH                      = vm.ProductNameTH     ?? string.Empty;
        product.NameEN                      = vm.ProductNameEN      ?? string.Empty;
        product.Slug                        = vm.Slug;
        product.Brand                       = vm.Brand;
        product.Model                       = vm.Model;
        product.Sku                         = vm.PartNumber;
        product.ShortDescriptionTH          = vm.ShortDescriptionTH;
        product.ShortDescriptionEN          = vm.ShortDescriptionEN;
        product.FullDescriptionTH           = SanitizedOrNull(vm.DescriptionTH);
        product.FullDescriptionEN           = SanitizedOrNull(vm.DescriptionEN);
        product.SpecificationTH             = SanitizedOrNull(vm.SpecificationTH);
        product.SpecificationEN             = SanitizedOrNull(vm.SpecificationEN);
        product.Price                       = vm.Price;
        product.IsPriceVisible              = vm.IsPriceVisible;
        product.DisplayOrder                = vm.DisplayOrder;
        product.IsFeatured                  = vm.IsFeatured;
        product.IsPromotion                 = vm.IsPromotion;
        product.PromotionLabelTH            = vm.PromotionLabelTH;
        product.PromotionLabelEN            = vm.PromotionLabelEN;
        product.IsActive                    = vm.IsActive;
        product.ShowYoutubeVideo            = vm.ShowYoutubeVideo;
        product.YoutubeVideoUrl             = string.IsNullOrWhiteSpace(vm.YoutubeVideoUrl) ? null : vm.YoutubeVideoUrl.Trim();
        product.YoutubeVideoTitleTH         = vm.YoutubeVideoTitleTH;
        product.YoutubeVideoTitleEN         = vm.YoutubeVideoTitleEN;
        product.YoutubeVideoDescriptionTH   = vm.YoutubeVideoDescriptionTH;
        product.YoutubeVideoDescriptionEN   = vm.YoutubeVideoDescriptionEN;
        product.UpdatedAt                   = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Product updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // ── Delete ──────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null)
        {
            TempData["Success"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product deleted successfully.";
        }
        catch (DbUpdateException)
        {
            product.IsActive  = false;
            product.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Warning"] = "Product has related data, so it was deactivated instead of deleted.";
        }

        return RedirectToAction(nameof(Index));
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    // Description/Specification are saved as sanitized HTML so the DB never stores the raw
    // execCommand output (style attributes, empty <li>/<p> junk) the WYSIWYG editor can produce.
    private string? SanitizedOrNull(string? html)
    {
        var safe = _richContent.ToSafeHtml(html);
        return string.IsNullOrWhiteSpace(safe) ? null : safe;
    }

    private static string NormalizeSlug(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var slug = input.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"-{2,}", "-");
        return slug.Trim('-');
    }

    private async Task LoadCategoriesAsync(int? selectedId)
    {
        var cats = await _db.ProductCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEN)
            .Select(c => new { c.Id, c.NameEN })
            .ToListAsync();

        ViewBag.Categories = new SelectList(cats, "Id", "NameEN", selectedId);
    }

    private async Task<(bool ok, string error, string path)> SaveImageAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedImageExtensions.Contains(ext))
            return (false,
                $"File type '{ext}' is not allowed. Accepted: jpg, jpeg, png, webp.",
                string.Empty);

        if (file.Length > MaxImageSize)
            return (false, "Image size must not exceed 5 MB.", string.Empty);

        var dir = Path.Combine(_env.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (true, string.Empty, $"/uploads/products/{fileName}");
    }

    private async Task<(bool ok, string error, string path)> SaveBrochureAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedBrochureExtensions.Contains(ext))
            return (false, "Only PDF files are allowed for brochures.", string.Empty);

        if (file.Length > MaxBrochureSize)
            return (false, "Brochure size must not exceed 10 MB.", string.Empty);

        var dir = Path.Combine(_env.WebRootPath, "uploads", "brochures");
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(dir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (true, string.Empty, $"/uploads/brochures/{fileName}");
    }

    private static ProductViewModel MapToViewModel(Product p) => new()
    {
        ProductId                   = p.Id,
        ProductCategoryId           = p.CategoryId,
        ProductNameTH               = p.NameTH,
        ProductNameEN               = p.NameEN,
        Slug                        = p.Slug,
        Brand                       = p.Brand,
        Model                       = p.Model,
        PartNumber                  = p.Sku,
        ShortDescriptionTH          = p.ShortDescriptionTH,
        ShortDescriptionEN          = p.ShortDescriptionEN,
        DescriptionTH               = p.FullDescriptionTH,
        DescriptionEN               = p.FullDescriptionEN,
        SpecificationTH             = p.SpecificationTH,
        SpecificationEN             = p.SpecificationEN,
        Price                       = p.Price,
        IsPriceVisible              = p.IsPriceVisible,
        MainImageUrl                = p.MainImagePath,
        BrochureFileUrl             = p.BrochureFilePath,
        DisplayOrder                = p.DisplayOrder,
        IsFeatured                  = p.IsFeatured,
        IsPromotion                 = p.IsPromotion,
        PromotionLabelTH            = p.PromotionLabelTH,
        PromotionLabelEN            = p.PromotionLabelEN,
        IsActive                    = p.IsActive,
        ShowYoutubeVideo            = p.ShowYoutubeVideo,
        YoutubeVideoUrl             = p.YoutubeVideoUrl,
        YoutubeVideoTitleTH         = p.YoutubeVideoTitleTH,
        YoutubeVideoTitleEN         = p.YoutubeVideoTitleEN,
        YoutubeVideoDescriptionTH   = p.YoutubeVideoDescriptionTH,
        YoutubeVideoDescriptionEN   = p.YoutubeVideoDescriptionEN,
    };
}
