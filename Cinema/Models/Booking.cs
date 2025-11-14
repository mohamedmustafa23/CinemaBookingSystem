using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

        [Required]
        public int ShowTimeId { get; set; }
        public ShowTime? ShowTime { get; set; }

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string BookingStatus { get; set; } = "Confirmed"; // Confirmed, Cancelled, Completed

        [Required]
        [MaxLength(100)]
        public string BookingReference { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Paid"; // Paid, Pending, Refunded

        public DateTime? PaymentDate { get; set; }

        public int NumberOfSeats { get; set; }

        public string? Notes { get; set; }

        // Navigation property
        public ICollection<BookingSeat> BookingSeats { get; set; } = new List<BookingSeat>();
    }
}