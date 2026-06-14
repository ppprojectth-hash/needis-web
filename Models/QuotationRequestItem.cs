using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class QuotationRequestItem
{
    public int Id { get; set; }

    public int QuotationRequestId { get; set; }

    [MaxLength(50)]
    public string ItemType { get; set; } = "Product";

    // ── Product fields ──────────────────────────────────────────────────────
    public int? ProductId { get; set; }

    [MaxLength(300)]
    public string? ProductNameSnapshotTH { get; set; }

    [MaxLength(300)]
    public string? ProductNameSnapshotEN { get; set; }

    [MaxLength(300)]
    public string? ProductSlugSnapshot { get; set; }

    [MaxLength(200)]
    public string? BrandSnapshot { get; set; }

    [MaxLength(200)]
    public string? ModelSnapshot { get; set; }

    [MaxLength(100)]
    public string? PartNumberSnapshot { get; set; }

    public int Quantity { get; set; } = 1;

    [MaxLength(500)]
    public string? ItemNote { get; set; }

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

    public DateTime CreatedAt { get; set; }

    public QuotationRequest QuotationRequest { get; set; } = null!;

    public Product? Product { get; set; }

    public Service? ServiceItem { get; set; }
}
