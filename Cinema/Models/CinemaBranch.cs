using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class CinemaBranch
    {
        public int Id { get; set; }

        [Required, MaxLength(100),MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(200),MinLength(3)]
        public string Location { get; set; } = string.Empty;

        public ICollection<CinemaHall> CinemaHalls { get; set; } = new List<CinemaHall>();
    }
}
