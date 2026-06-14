using System.ComponentModel.DataAnnotations;

namespace Needis.Web.Models;

public class ActivityTag
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string TagKey { get; set; } = string.Empty;

    [MaxLength(200)] public string? TagNameTH { get; set; }
    [MaxLength(200)] public string? TagNameEN { get; set; }
    [MaxLength(500)] public string? TagDescriptionTH { get; set; }
    [MaxLength(500)] public string? TagDescriptionEN { get; set; }
    [MaxLength(50)]  public string? TagColor { get; set; }

    public int  DisplayOrder  { get; set; }
    public bool IsFilterable  { get; set; } = true;
    public bool IsActive      { get; set; } = true;
    public bool IsDelete      { get; set; } = false;

    public DateTime  CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
    [MaxLength(100)] public string? CreatedBy { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }

    // Navigation
    public ICollection<ActivityTagMap> ActivityTagMaps { get; set; } = new List<ActivityTagMap>();
}
