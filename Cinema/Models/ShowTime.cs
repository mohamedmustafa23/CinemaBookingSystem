using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class ShowTime
    {
        public int Id { get; set; }
        [Required]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }
        [Required]
        public int CinemaHallId { get; set; }
        public CinemaHall? CinemaHall { get; set; }
        [Required]
        public DateTime ShowDate { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TicketPrice { get; set; }

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }
    }

}
