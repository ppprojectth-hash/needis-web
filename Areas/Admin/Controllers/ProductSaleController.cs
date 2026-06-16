using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Helpers;
using Needis.Web.Models;
using Needis.Web.ViewModels.Admin;

namespace Needis.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[RequirePermission("About.View")]
public class ProductSaleController : Controller
{
    private readonly AppDbContext _db;
    private string CurrentUser => User.Identity?.Name ?? "system";

    public ProductSaleController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index(DateTime? dateFrom, DateTime? dateTo, string? keyword)
    {
        ViewData["Title"] = "Product Sales";

        var nowBangkok = BangkokTimeHelper.NowBangkok();
        dateFrom ??= nowBangkok.Date.AddMonths(-1);
        dateTo   ??= nowBangkok.Date;

        ViewBag.DateFrom = dateFrom.Value.ToString("yyyy-MM-dd");
        ViewBag.DateTo   = dateTo.Value.ToString("yyyy-MM-dd");
        ViewBag.Keyword  = keyword;

        var from        = BangkokTimeHelper.ConvertBangkokDateStartToUtc(dateFrom.Value);
        var toExclusive = BangkokTimeHelper.ConvertBangkokDateEndExclusiveToUtc(dateTo.Value);

        var query = _db.ProductSales.AsNoTracking()
            .Include(ps => ps.Product)
            .Where(ps => !ps.IsDelete && ps.SaleDate >= from && ps.SaleDate < toExclusive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(ps =>
                (ps.Product != null && (ps.Product.NameEN.Contains(keyword) || ps.Product.NameTH.Contains(keyword))) ||
                (ps.CustomerName != null && ps.CustomerName.Contains(keyword)) ||
                (ps.ReferenceNo  != null && ps.ReferenceNo.Contains(keyword)));

        var list = await query
            .OrderByDescending(ps => ps.SaleDate)
            .ToListAsync();

        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Title"] = "Add Product Sale";
        await LoadProductsAsync(null);
        return View(new ProductSaleViewModel { SaleDate = DateTime.Today, Quantity = 1, CountInAboutStats = true, IsActive = true });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductSaleViewModel vm)
    {
        ViewData["Title"] = "Add Product Sale";

        if (!ModelState.IsValid)
        {
            await LoadProductsAsync(vm.ProductId);
            return View(vm);
        }

        _db.ProductSales.Add(new ProductSale
        {
            ProductId         = vm.ProductId,
            SaleDate          = DateTime.SpecifyKind(vm.SaleDate.Date, DateTimeKind.Utc),
            Quantity          = vm.Quantity,
            CustomerName      = vm.CustomerName,
            ReferenceNo       = vm.ReferenceNo,
            CountInAboutStats = vm.CountInAboutStats,
            IsActive          = vm.IsActive,
            CreatedAt         = DateTime.UtcNow,
            CreatedBy         = CurrentUser,
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Product sale added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["Title"] = "Edit Product Sale";
        var ps = await _db.ProductSales.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        if (ps is null) return NotFound();

        await LoadProductsAsync(ps.ProductId);
        return View(new ProductSaleViewModel
        {
            Id                = ps.Id,
            ProductId         = ps.ProductId,
            SaleDate          = ps.SaleDate,
            Quantity          = ps.Quantity,
            CustomerName      = ps.CustomerName,
            ReferenceNo       = ps.ReferenceNo,
            CountInAboutStats = ps.CountInAboutStats,
            IsActive          = ps.IsActive,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductSaleViewModel vm)
    {
        ViewData["Title"] = "Edit Product Sale";

        if (!ModelState.IsValid)
        {
            await LoadProductsAsync(vm.ProductId);
            return View(vm);
        }

        var entity = await _db.ProductSales.FindAsync(id);
        if (entity is null || entity.IsDelete) return NotFound();

        entity.ProductId         = vm.ProductId;
        entity.SaleDate          = DateTime.SpecifyKind(vm.SaleDate.Date, DateTimeKind.Utc);
        entity.Quantity          = vm.Quantity;
        entity.CustomerName      = vm.CustomerName;
        entity.ReferenceNo       = vm.ReferenceNo;
        entity.CountInAboutStats = vm.CountInAboutStats;
        entity.IsActive          = vm.IsActive;
        entity.UpdatedAt         = DateTime.UtcNow;
        entity.UpdatedBy         = CurrentUser;

        await _db.SaveChangesAsync();
        TempData["Success"] = "Product sale updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _db.ProductSales.FindAsync(id);
        if (entity is not null)
        {
            entity.IsDelete  = true;
            entity.IsActive  = false;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = CurrentUser;
            await _db.SaveChangesAsync();
            TempData["Success"] = "Product sale deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadProductsAsync(int? selectedId)
    {
        var products = await _db.Products.AsNoTracking()
            .Where(p => p.IsActive)
            .OrderBy(p => p.NameEN)
            .Select(p => new
            {
                p.Id,
                DisplayName = p.NameEN +
                    (p.Brand != null ? $" [{p.Brand}]" : "") +
                    (p.Model != null ? $" - {p.Model}" : ""),
            })
            .ToListAsync();

        ViewBag.Products = new SelectList(products, "Id", "DisplayName", selectedId);
    }
}
