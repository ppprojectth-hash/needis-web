using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ContactMessage
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Phone { get; set; }

    [Required, MaxLength(300)]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    [MaxLength(50)]
    public string Status { get; set; } = "New";

    public string? AdminRemark { get; set; }

    [MaxLength(150)]
    public string? AssignedTo { get; set; }

    public DateTime? ReadAt    { get; set; }
    public DateTime? RepliedAt { get; set; }
    public DateTime? ClosedAt  { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [MaxLength(10)]
    public string? Language { get; set; }

    public DateTime  CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(150)]
    public string? UpdatedBy { get; set; }
}
