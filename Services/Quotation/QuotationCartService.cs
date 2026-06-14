using Microsoft.EntityFrameworkCore;
using Needis.Web.Data;
using Needis.Web.Models;

namespace Needis.Web.Services.Quotation;

public class QuotationCartService : IQuotationCartService
{
    private const string CookieName = "Needis.QuotationCart";
    private static readonly TimeSpan CartLifetime = TimeSpan.FromDays(30);

    private readonly AppDbContext _db;
    private readonly ILogger<QuotationCartService> _logger;

    public QuotationCartService(AppDbContext db, ILogger<QuotationCartService> logger)
    {
        _db     = db;
        _logger = logger;
    }

    // ── Cookie helpers ───────────────────────────────────────────────────────

    private static string GetOrSetToken(HttpContext ctx)
    {
        if (ctx.Request.Cookies.TryGetValue(CookieName, out var token) &&
            !string.IsNullOrWhiteSpace(token))
            return token;

        token = Guid.NewGuid().ToString("N");
        SetCookie(ctx, token);
        return token;
    }

    private static void SetCookie(HttpContext ctx, string token)
    {
        ctx.Response.Cookies.Append(CookieName, token, new CookieOptions
        {
            Expires   = DateTimeOffset.UtcNow.Add(CartLifetime),
            HttpOnly  = true,
            SameSite  = SameSiteMode.Lax,
            Secure    = ctx.Request.IsHttps,
        });
    }

    // ── GetOrCreateCartAsync ─────────────────────────────────────────────────

    public async Task<QuotationCart> GetOrCreateCartAsync(HttpContext ctx)
    {
        var token = GetOrSetToken(ctx);
        var now   = DateTime.UtcNow;

        var cart = await _db.QuotationCarts
            .FirstOrDefaultAsync(c => c.CartToken == token);

        // Recycle: submitted, expired, or not found → new cart
        if (cart is null || cart.IsSubmitted || (cart.ExpiresAt.HasValue && cart.ExpiresAt < now))
        {
            // Clean up expired non-submitted carts silently
            if (cart is not null && !cart.IsSubmitted && cart.ExpiresAt.HasValue && cart.ExpiresAt < now)
            {
                try { _db.QuotationCarts.Remove(cart); await _db.SaveChangesAsync(); }
                catch (Exception ex) { _logger.LogWarning(ex, "Failed to remove expired cart"); }
            }

            token = Guid.NewGuid().ToString("N");
            SetCookie(ctx, token);

            cart = new QuotationCart
            {
                CartToken = token,
                Language  = ctx.Request.Cookies["Needis.Language"] ?? "en",
                IpAddress = ctx.Connection.RemoteIpAddress?.ToString(),
                UserAgent = ctx.Request.Headers.UserAgent.FirstOrDefault()?.Length > 500
                    ? ctx.Request.Headers.UserAgent.FirstOrDefault()![..500]
                    : ctx.Request.Headers.UserAgent.FirstOrDefault(),
                CreatedAt = now,
                ExpiresAt = now.Add(CartLifetime),
            };
            _db.QuotationCarts.Add(cart);
            await _db.SaveChangesAsync();
        }

        return cart;
    }

    // ── GetActiveCartWithItemsAsync ──────────────────────────────────────────

    public async Task<QuotationCart?> GetActiveCartWithItemsAsync(HttpContext ctx)
    {
        if (!ctx.Request.Cookies.TryGetValue(CookieName, out var token) ||
            string.IsNullOrWhiteSpace(token))
            return null;

        var now = DateTime.UtcNow;
        return await _db.QuotationCarts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c =>
                c.CartToken == token &&
                !c.IsSubmitted &&
                (c.ExpiresAt == null || c.ExpiresAt > now));
    }

    // ── GetCartItemCountAsync ────────────────────────────────────────────────

    public async Task<int> GetCartItemCountAsync(HttpContext ctx)
    {
        if (!ctx.Request.Cookies.TryGetValue(CookieName, out var token) ||
            string.IsNullOrWhiteSpace(token))
            return 0;

        var now = DateTime.UtcNow;
        var cart = await _db.QuotationCarts
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.CartToken == token &&
                !c.IsSubmitted &&
                (c.ExpiresAt == null || c.ExpiresAt > now));

        if (cart is null) return 0;

        return await _db.QuotationCartItems
            .AsNoTracking()
            .CountAsync(i => i.QuotationCartId == cart.Id);
    }

    // ── AddProductAsync ──────────────────────────────────────────────────────

    public async Task AddProductAsync(HttpContext ctx, int productId, int quantity, string? itemNote)
    {
        var product = await _db.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

        if (product is null) return;

        var cart = await GetOrCreateCartAsync(ctx);

        // Check for existing item with same product
        var existing = await _db.QuotationCartItems
            .FirstOrDefaultAsync(i => i.QuotationCartId == cart.Id &&
                                      i.ItemType == "Product" &&
                                      i.ProductId == productId);
        if (existing is not null)
        {
            existing.Quantity  += Math.Max(1, quantity);
            existing.UpdatedAt  = DateTime.UtcNow;
        }
        else
        {
            _db.QuotationCartItems.Add(new QuotationCartItem
            {
                QuotationCartId       = cart.Id,
                ItemType              = "Product",
                ProductId             = product.Id,
                ProductNameSnapshotTH = product.NameTH,
                ProductNameSnapshotEN = product.NameEN,
                ProductSlugSnapshot   = product.Slug,
                BrandSnapshot         = product.Brand,
                ModelSnapshot         = product.Model,
                PartNumberSnapshot    = product.Sku,
                Quantity              = Math.Max(1, quantity),
                ItemNote              = itemNote,
                CreatedAt             = DateTime.UtcNow,
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── AddServiceAsync ──────────────────────────────────────────────────────

    public async Task AddServiceAsync(HttpContext ctx, int serviceId, int quantity, string? itemNote)
    {
        var service = await _db.Services
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive && !s.IsDelete);

        if (service is null) return;

        var cart = await GetOrCreateCartAsync(ctx);

        var existing = await _db.QuotationCartItems
            .FirstOrDefaultAsync(i => i.QuotationCartId == cart.Id &&
                                      i.ItemType == "Service" &&
                                      i.ServiceId == serviceId);
        if (existing is not null)
        {
            existing.Quantity  += Math.Max(1, quantity);
            existing.UpdatedAt  = DateTime.UtcNow;
        }
        else
        {
            _db.QuotationCartItems.Add(new QuotationCartItem
            {
                QuotationCartId      = cart.Id,
                ItemType             = "Service",
                ServiceId            = service.Id,
                ServiceCodeSnapshot  = service.ServiceCode,
                ServiceNameSnapshotTH = service.ServiceNameTH,
                ServiceNameSnapshotEN = service.ServiceNameEN,
                ServiceSlugSnapshot  = service.ServiceSlug,
                Quantity             = Math.Max(1, quantity),
                ItemNote             = itemNote,
                CreatedAt            = DateTime.UtcNow,
            });
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── UpdateQuantityAsync ──────────────────────────────────────────────────

    public async Task UpdateQuantityAsync(HttpContext ctx, int itemId, int quantity)
    {
        var cart = await GetActiveCartAsync(ctx);
        if (cart is null) return;

        var item = await _db.QuotationCartItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.QuotationCartId == cart.Id);

        if (item is null) return;

        item.Quantity  = Math.Max(1, quantity);
        item.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── UpdateNoteAsync ──────────────────────────────────────────────────────

    public async Task UpdateNoteAsync(HttpContext ctx, int itemId, string? itemNote)
    {
        var cart = await GetActiveCartAsync(ctx);
        if (cart is null) return;

        var item = await _db.QuotationCartItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.QuotationCartId == cart.Id);

        if (item is null) return;

        item.ItemNote  = itemNote;
        item.UpdatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── RemoveItemAsync ──────────────────────────────────────────────────────

    public async Task RemoveItemAsync(HttpContext ctx, int itemId)
    {
        var cart = await GetActiveCartAsync(ctx);
        if (cart is null) return;

        var item = await _db.QuotationCartItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.QuotationCartId == cart.Id);

        if (item is null) return;

        _db.QuotationCartItems.Remove(item);
        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── ClearCartAsync ───────────────────────────────────────────────────────

    public async Task ClearCartAsync(HttpContext ctx)
    {
        var cart = await GetActiveCartAsync(ctx);
        if (cart is null) return;

        var items = await _db.QuotationCartItems
            .Where(i => i.QuotationCartId == cart.Id)
            .ToListAsync();

        _db.QuotationCartItems.RemoveRange(items);
        cart.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    // ── Private helper ───────────────────────────────────────────────────────

    private async Task<QuotationCart?> GetActiveCartAsync(HttpContext ctx)
    {
        if (!ctx.Request.Cookies.TryGetValue(CookieName, out var token) ||
            string.IsNullOrWhiteSpace(token))
            return null;

        var now = DateTime.UtcNow;
        return await _db.QuotationCarts
            .FirstOrDefaultAsync(c =>
                c.CartToken == token &&
                !c.IsSubmitted &&
                (c.ExpiresAt == null || c.ExpiresAt > now));
    }
}
