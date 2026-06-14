using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class EmailSendLog
{
    public int Id { get; set; }

    [MaxLength(100)]
    public string? EmailType { get; set; }

    [MaxLength(300)]
    public string? ToEmail { get; set; }

    [MaxLength(500)]
    public string? Subject { get; set; }

    [MaxLength(100)]
    public string? ReferenceType { get; set; }

    public int? ReferenceId { get; set; }

    [MaxLength(50)]
    public string? Status { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }
}
