using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public DateTime? Birthday { get; set; }
        [NotMapped]
        public string Role { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; }
    }
}
