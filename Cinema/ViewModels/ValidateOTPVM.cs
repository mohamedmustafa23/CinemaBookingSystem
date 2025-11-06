using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModels
{
    public class ValidateOTPVM
    {
        [Required]
        public string ApplicationUserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        public string OTP { get; set; }
    
        public string Email { get; set; } = string.Empty;
    }
}