namespace Cinema.ViewModels
{
    public class SeatAvailabilityVM
    {
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
