using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class CinemaHall
    {
        public int Id { get; set; }

        [Required, MaxLength(100),MinLength(3)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; }

        [Required]
        public string HallType { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = string.Empty;
        [Required]
        public int BranchId { get; set; }
        public CinemaBranch? Branch { get; set; }

        public ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
    }
}
