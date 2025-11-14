using Cinema.Models;

namespace Cinema.ViewModels
{
    public class MovieBookingVM
    {
        public Movie Movie { get; set; } = null!;
        public List<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
        public List<DateTime> AvailableDates { get; set; } = new List<DateTime>();

        // معلومات الحجز المختارة
        public int? SelectedShowTimeId { get; set; }
        public DateTime? SelectedDate { get; set; }
        public List<string> SelectedSeats { get; set; } = new List<string>();
        public decimal TotalPrice { get; set; }
    }
}