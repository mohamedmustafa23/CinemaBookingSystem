namespace Cinema.ViewModels
{
    public class ShowTimeDetailsVM
    {
        public int Id { get; set; }
        public string CinemaHallName { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public decimal TicketPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ShowDate { get; set; }
    }
}
