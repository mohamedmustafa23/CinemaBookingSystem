using System.ComponentModel.DataAnnotations;

namespace Cinema.ViewModels
{
    public class BookingConfirmVM
    {
        [Required]
        public int ShowTimeId { get; set; }

        [Required]
        public List<string> SelectedSeats { get; set; } = new List<string>();

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // Card details (if payment method is card)
        public string? CardHolderName { get; set; }
        public string? CardNumber { get; set; }
        public string? ExpiryMonth { get; set; }
        public string? ExpiryYear { get; set; }
        public string? CVV { get; set; }
    }
}
