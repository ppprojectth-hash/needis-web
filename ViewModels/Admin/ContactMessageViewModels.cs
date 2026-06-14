using System.ComponentModel.DataAnnotations;
using Needis.Web.Models;

namespace Needis.Web.ViewModels.Admin;

public class ContactMessageIndexViewModel
{
    public DateTime  DateFrom    { get; set; }
    public DateTime  DateTo      { get; set; }
    public string?   Status      { get; set; }
    public string?   Keyword     { get; set; }
    public string?   AssignedTo  { get; set; }

    public List<ContactMessageListItemViewModel> Messages { get; set; } = [];

    public int TotalCount   { get; set; }
    public int NewCount     { get; set; }
    public int ReadCount    { get; set; }
    public int RepliedCount { get; set; }
    public int ClosedCount  { get; set; }
    public int SpamCount    { get; set; }
}

public class ContactMessageListItemViewModel
{
    public int       Id         { get; set; }
    public string?   FullName   { get; set; }
    public string?   Email      { get; set; }
    public string?   Phone      { get; set; }
    public string?   Subject    { get; set; }
    public string    Status     { get; set; } = "New";
    public string?   AssignedTo { get; set; }
    public DateTime  CreatedAt  { get; set; }
    public DateTime? ReadAt     { get; set; }
    public DateTime? RepliedAt  { get; set; }
    public DateTime? ClosedAt   { get; set; }
}

public class ContactMessageDetailViewModel
{
    public ContactMessage Message     { get; set; } = null!;
    public string?        NewStatus   { get; set; }
    public string?        AdminRemark { get; set; }
    public string?        AssignedTo  { get; set; }
}

public class ContactMessageUpdateViewModel
{
    public int Id { get; set; }

    [Required]
    public string Status { get; set; } = "New";

    public string? AdminRemark { get; set; }

    [MaxLength(150)]
    public string? AssignedTo { get; set; }
}
