using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Services;
using Needis.Web.ViewModels.Product;

namespace Needis.Web.Controllers;

public class ProductController : Controller
{
    private readonly AppDbContext     _db;
    private readonly ILanguageService _lang;

    public ProductController(AppDbContext db, ILanguageService lang)
    {
        _db   = db;
        _lang = lang;
    }

    // GET /Product  or  /Product?category={slug}&search={q}
    public async Task<IActionResult> Index(string? category, string? search)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        var categories = await _db.ProductCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.NameEN)
            .ToListAsync();

        Models.ProductCategory? selectedCategory = null;
        if (!string.IsNullOrWhiteSpace(category))
            selectedCategory = categories.FirstOrDefault(c => c.Slug == category);

        var query = _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (selectedCategory != null)
            query = query.Where(p => p.CategoryId == selectedCategory.Id);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(p =>
                p.NameEN.ToLower().Contains(s) ||
                p.NameTH.ToLower().Contains(s) ||
                (p.Brand              != null && p.Brand.ToLower().Contains(s))              ||
                (p.Model              != null && p.Model.ToLower().Contains(s))              ||
                (p.Sku                != null && p.Sku.ToLower().Contains(s))                ||
                (p.ShortDescriptionEN != null && p.ShortDescriptionEN.ToLower().Contains(s)) ||
                (p.ShortDescriptionTH != null && p.ShortDescriptionTH.ToLower().Contains(s)));
        }

        var products = await query
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.NameEN)
            .ToListAsync();

        var vm = new ProductListViewModel
        {
            Products             = products,
            Categories           = categories,
            SelectedCategorySlug = selectedCategory?.Slug,
            SelectedCategoryId   = selectedCategory?.Id,
            SelectedCategory     = selectedCategory,
            Search               = search?.Trim(),
            CurrentLanguage      = lang,
        };

        if (selectedCategory != null)
        {
            ViewData["SeoPageKey"]    = "products";
            ViewData["SeoEntityType"] = "ProductCategory";
            ViewData["SeoEntityId"]   = selectedCategory.Id;
            ViewData["SeoTitleTH"]    = selectedCategory.NameTH;
            ViewData["SeoTitleEN"]    = selectedCategory.NameEN;
            ViewData["SeoDescriptionTH"] = selectedCategory.ShortDescriptionTH;
            ViewData["SeoDescriptionEN"] = selectedCategory.ShortDescriptionEN;
        }
        else
        {
            ViewData["SeoPageKey"] = "products";
        }
        return View(vm);
    }

    // GET /Product/Detail/{slug}
    public async Task<IActionResult> Detail(string slug)
    {
        var lang = _lang.GetCurrentLanguage(HttpContext);

        var product = await _db.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

        if (product is null) return NotFound();

        var related = await _db.Products
            .AsNoTracking()
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
            .OrderBy(p => p.DisplayOrder)
            .ThenBy(p => p.NameEN)
            .Take(4)
            .ToListAsync();

        var specs = await _db.ProductSpecifications
            .AsNoTracking()
            .Where(s => s.ProductId == product.Id && s.IsActive && !s.IsDelete)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Id)
            .ToListAsync();

        var vm = new ProductDetailViewModel
        {
            Product         = product,
            RelatedProducts = related,
            Specifications  = specs,
            CurrentLanguage = lang,
        };

        ViewData["SeoPageKey"]       = "product-detail";
        ViewData["SeoEntityType"]    = "Product";
        ViewData["SeoEntityId"]      = product.Id;
        ViewData["SeoTitleTH"]       = product.NameTH;
        ViewData["SeoTitleEN"]       = product.NameEN;
        ViewData["SeoDescriptionTH"] = product.ShortDescriptionTH;
        ViewData["SeoDescriptionEN"] = product.ShortDescriptionEN;
        ViewData["SeoImageUrl"]      = product.MainImagePath;
        return View(vm);
    }
}
