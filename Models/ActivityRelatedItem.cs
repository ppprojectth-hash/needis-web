using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ActivityRelatedItem
{
    public int  Id                { get; set; }
    public int  ActivityId        { get; set; }
    public int  RelatedActivityId { get; set; }
    public int  DisplayOrder      { get; set; }
    public bool IsActive          { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    [MaxLength(100)] public string? CreatedBy { get; set; }

    // Navigation
    public Activity? Activity        { get; set; }
    public Activity? RelatedActivity { get; set; }
}
