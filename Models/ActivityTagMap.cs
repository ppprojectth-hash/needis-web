using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ActivityTagMap
{
    public int  Id            { get; set; }
    public int  ActivityId    { get; set; }
    public int  ActivityTagId { get; set; }
    public bool IsPrimary     { get; set; } = false;
    public int  DisplayOrder  { get; set; }

    public DateTime CreatedAt { get; set; }
    [MaxLength(100)] public string? CreatedBy { get; set; }

    // Navigation
    public Activity?    Activity    { get; set; }
    public ActivityTag? ActivityTag { get; set; }
}
