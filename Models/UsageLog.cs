using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class UsageLog
{
    public long Id { get; set; }

    [Required, MaxLength(500)]
    public string PageUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Path { get; set; }

    [MaxLength(1000)]
    public string? QueryString { get; set; }

    [MaxLength(200)]
    public string? PageName { get; set; }

    [MaxLength(200)]
    public string? FunctionName { get; set; }

    [MaxLength(10)]
    public string? HttpMethod { get; set; }

    [MaxLength(50)]
    public string? Area { get; set; }

    [MaxLength(100)]
    public string? Controller { get; set; }

    [MaxLength(100)]
    public string? Action { get; set; }

    [MaxLength(100)]
    public string? UserId { get; set; }

    [MaxLength(200)]
    public string? Username { get; set; }

    public int? AdminUserId { get; set; }

    [MaxLength(50)]
    public string? IpAddress { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    [MaxLength(500)]
    public string? Referrer { get; set; }

    [MaxLength(5)]
    public string? Language { get; set; }

    public int StatusCode { get; set; }

    public bool IsSuccess { get; set; } = true;

    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    public long DurationMs { get; set; }

    public DateTime AccessedAt { get; set; }

    public AdminUser? AdminUser { get; set; }
}
