using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class BookingSeat
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        [Required]
        [MaxLength(10)]
        public string SeatNumber { get; set; } = string.Empty; // e.g., "A1", "B5"

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatStatus { get; set; } = "Booked"; // Booked, Cancelled

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
