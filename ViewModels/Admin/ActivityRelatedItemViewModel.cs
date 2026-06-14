using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Admin;

public class ActivityRelatedItemViewModel
{
    public int Id { get; set; }

    [Required]
    public int ActivityId { get; set; }

    [Required]
    public int RelatedActivityId { get; set; }

    public int  DisplayOrder { get; set; }
    public bool IsActive     { get; set; } = true;
}
