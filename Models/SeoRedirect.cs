using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class SeoRedirect
{
    public int Id { get; set; }

    [Required, MaxLength(500)]
    public string SourcePath { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string TargetPath { get; set; } = string.Empty;

    public int StatusCode { get; set; } = 301;

    public bool IsActive  { get; set; } = true;
    public bool IsDelete  { get; set; } = false;

    public DateTime  CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? CreatedBy { get; set; }

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
