using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class QuotationCartItem
{
    public int Id { get; set; }

    public int QuotationCartId { get; set; }

    [Required, MaxLength(50)]
    public string ItemType { get; set; } = "Product";

    // ── Product fields ──────────────────────────────────────────────────────
    public int? ProductId { get; set; }

    [MaxLength(200)]
    public string? ProductNameSnapshotTH { get; set; }

    [MaxLength(200)]
    public string? ProductNameSnapshotEN { get; set; }

    [MaxLength(150)]
    public string? ProductSlugSnapshot { get; set; }

    [MaxLength(100)]
    public string? BrandSnapshot { get; set; }

    [MaxLength(100)]
    public string? ModelSnapshot { get; set; }

    [MaxLength(100)]
    public string? PartNumberSnapshot { get; set; }

    // ── Service fields ──────────────────────────────────────────────────────
    public int? ServiceId { get; set; }

    [MaxLength(100)]
    public string? ServiceCodeSnapshot { get; set; }

    [MaxLength(200)]
    public string? ServiceNameSnapshotTH { get; set; }

    [MaxLength(200)]
    public string? ServiceNameSnapshotEN { get; set; }

    [MaxLength(150)]
    public string? ServiceSlugSnapshot { get; set; }

    // ── Common ──────────────────────────────────────────────────────────────
    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? ItemNote { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // ── Navigation ──────────────────────────────────────────────────────────
    public QuotationCart? QuotationCart { get; set; }

    public Product? Product { get; set; }

    public Service? Service { get; set; }
}
