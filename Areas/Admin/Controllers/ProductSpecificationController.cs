using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("Product.View")]
public class ProductSpecificationController : Controller
{
    private readonly AppDbContext _db;

    public ProductSpecificationController(AppDbContext db) => _db = db;

    // ── GET Index ────────────────────────────────────────────────────────────

    [HttpGet]
    public async Task<IActionResult> Index(int productId, string? keyword)
    {
        var product = await _db.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null) return NotFound();

        var query = _db.ProductSpecifications
            .AsNoTracking()
            .Where(s => s.ProductId == productId && !s.IsDelete)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = keyword.Trim();
            query = query.Where(s =>
                (s.SpecGroupTH  != null && s.SpecGroupTH.Contains(kw))  ||
                (s.SpecGroupEN  != null && s.SpecGroupEN.Contains(kw))  ||
                (s.SpecNameTH   != null && s.SpecNameTH.Contains(kw))   ||
                s.SpecNameEN.Contains(kw)                                ||
                (s.SpecValueTH  != null && s.SpecValueTH.Contains(kw))  ||
                (s.SpecValueEN  != null && s.SpecValueEN.Contains(kw)));
        }

        var specs = await query
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Id)
            .ToListAsync();

        ViewData["Title"] = $"Specifications — {product.NameEN}";

        return View(new ProductSpecificationIndexViewModel
        {
            ProductId     = productId,
            ProductNameEN = product.NameEN,
            ProductNameTH = product.NameTH,
            ProductSlug   = product.Slug,
            Keyword       = keyword,
            Specifications = specs,
        });
    }

    // ── GET Create ───────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Create(int productId)
    {
        var product = await _db.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null) return NotFound();

        ViewData["Title"] = $"Add Specification — {product.NameEN}";
        return View(new ProductSpecificationViewModel
        {
            ProductId     = productId,
            ProductNameEN = product.NameEN,
            ProductNameTH = product.NameTH,
            IsActive      = true,
        });
    }

    // ── POST Create ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Create(ProductSpecificationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Add Specification";
            return View(model);
        }

        var spec = new ProductSpecification
        {
            ProductId    = model.ProductId,
            SpecGroupTH  = model.SpecGroupTH?.Trim(),
            SpecGroupEN  = model.SpecGroupEN?.Trim(),
            SpecNameTH   = model.SpecNameTH?.Trim(),
            SpecNameEN   = model.SpecNameEN!.Trim(),
            SpecValueTH  = model.SpecValueTH?.Trim(),
            SpecValueEN  = model.SpecValueEN?.Trim(),
            UnitTH       = model.UnitTH?.Trim(),
            UnitEN       = model.UnitEN?.Trim(),
            RemarkTH     = model.RemarkTH?.Trim(),
            RemarkEN     = model.RemarkEN?.Trim(),
            DisplayOrder = model.DisplayOrder,
            IsHighlight  = model.IsHighlight,
            IsActive     = model.IsActive,
            CreatedAt    = DateTime.UtcNow,
            CreatedBy    = User.FindFirst(ClaimTypes.Name)?.Value,
        };

        _db.ProductSpecifications.Add(spec);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Specification '{spec.SpecNameEN}' added.";
        return RedirectToAction(nameof(Index), new { productId = model.ProductId });
    }

    // ── GET Edit ─────────────────────────────────────────────────────────────

    [HttpGet]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Edit(int id)
    {
        var spec = await _db.ProductSpecifications
            .AsNoTracking()
            .Include(s => s.Product)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);

        if (spec is null) return NotFound();

        ViewData["Title"] = $"Edit — {spec.SpecNameEN}";
        return View(new ProductSpecificationViewModel
        {
            Id           = spec.Id,
            ProductId    = spec.ProductId,
            ProductNameEN = spec.Product?.NameEN,
            ProductNameTH = spec.Product?.NameTH,
            SpecGroupTH  = spec.SpecGroupTH,
            SpecGroupEN  = spec.SpecGroupEN,
            SpecNameTH   = spec.SpecNameTH,
            SpecNameEN   = spec.SpecNameEN,
            SpecValueTH  = spec.SpecValueTH,
            SpecValueEN  = spec.SpecValueEN,
            UnitTH       = spec.UnitTH,
            UnitEN       = spec.UnitEN,
            RemarkTH     = spec.RemarkTH,
            RemarkEN     = spec.RemarkEN,
            DisplayOrder = spec.DisplayOrder,
            IsHighlight  = spec.IsHighlight,
            IsActive     = spec.IsActive,
        });
    }

    // ── POST Edit ────────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Edit(int id, ProductSpecificationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Title"] = "Edit Specification";
            return View(model);
        }

        var spec = await _db.ProductSpecifications
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);

        if (spec is null) return NotFound();

        spec.SpecGroupTH  = model.SpecGroupTH?.Trim();
        spec.SpecGroupEN  = model.SpecGroupEN?.Trim();
        spec.SpecNameTH   = model.SpecNameTH?.Trim();
        spec.SpecNameEN   = model.SpecNameEN!.Trim();
        spec.SpecValueTH  = model.SpecValueTH?.Trim();
        spec.SpecValueEN  = model.SpecValueEN?.Trim();
        spec.UnitTH       = model.UnitTH?.Trim();
        spec.UnitEN       = model.UnitEN?.Trim();
        spec.RemarkTH     = model.RemarkTH?.Trim();
        spec.RemarkEN     = model.RemarkEN?.Trim();
        spec.DisplayOrder = model.DisplayOrder;
        spec.IsHighlight  = model.IsHighlight;
        spec.IsActive     = model.IsActive;
        spec.UpdatedAt    = DateTime.UtcNow;
        spec.UpdatedBy    = User.FindFirst(ClaimTypes.Name)?.Value;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Specification '{spec.SpecNameEN}' updated.";
        return RedirectToAction(nameof(Index), new { productId = spec.ProductId });
    }

    // ── POST Delete ──────────────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> Delete(int id)
    {
        var spec = await _db.ProductSpecifications
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);

        if (spec is null) return NotFound();

        var productId = spec.ProductId;
        spec.IsDelete  = true;
        spec.IsActive  = false;
        spec.UpdatedAt = DateTime.UtcNow;
        spec.UpdatedBy = User.FindFirst(ClaimTypes.Name)?.Value;

        await _db.SaveChangesAsync();

        TempData["Success"] = "Specification deleted.";
        return RedirectToAction(nameof(Index), new { productId });
    }

    // ── POST ToggleHighlight ─────────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Product.Edit")]
    public async Task<IActionResult> ToggleHighlight(int id)
    {
        var spec = await _db.ProductSpecifications
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDelete);

        if (spec is null) return NotFound();

        spec.IsHighlight = !spec.IsHighlight;
        spec.UpdatedAt   = DateTime.UtcNow;
        spec.UpdatedBy   = User.FindFirst(ClaimTypes.Name)?.Value;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { productId = spec.ProductId });
    }
}
