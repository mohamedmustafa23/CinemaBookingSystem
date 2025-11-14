using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cinema.ViewModels
{
    public class ProfileVM
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Location { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? Birthday { get; set; }

        public string? ProfilePicture { get; set; } 
        public IFormFile? ProfileImage { get; set; } 
    }
}
