namespace Cinema.Models
{
    public class DashboardViewModel
    {
        public int MoviesCount { get; set; }
        public int HallsCount { get; set; }
        public int ActorsCount { get; set; }
        public int ShowtimesCount { get; set; }
        public int TodayShowtimesCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
