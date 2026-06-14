using System.ComponentModel.DataAnnotations;

namespace Needis.Web.ViewModels.Contact;

public class ContactFormViewModel
{
    [Required(ErrorMessage = "Full name is required.")]
    [Display(Name = "Full Name")]
    [MaxLength(200)]
    public string? FullName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [Display(Name = "Email")]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Display(Name = "Phone")]
    [MaxLength(50)]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Subject is required.")]
    [Display(Name = "Subject")]
    [MaxLength(300)]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Message is required.")]
    [Display(Name = "Message")]
    public string? Message { get; set; }
}
