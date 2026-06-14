using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ActivityTagViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string TagKey { get; set; } = string.Empty;

    [MaxLength(200)] public string? TagNameTH { get; set; }
    [Required, MaxLength(200)] public string? TagNameEN { get; set; }
    [MaxLength(500)] public string? TagDescriptionTH { get; set; }
    [MaxLength(500)] public string? TagDescriptionEN { get; set; }
    [MaxLength(50)]  public string? TagColor { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsFilterable { get; set; } = true;
    public bool IsActive     { get; set; } = true;
}
