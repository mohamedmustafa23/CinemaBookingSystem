using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModels
{
    public class ForgotPasswordVM
{
    [Required(ErrorMessage = "Email or Username is required")]
    [Display(Name = "Email or Username")]
    public string UserNameOrEmail { get; set; } = string.Empty;
    }
}
