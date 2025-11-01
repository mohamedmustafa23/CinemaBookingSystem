using Cinema.Models;

namespace Cinema.ViewModels
{
    public class ShowTimeVM
    {
        public ShowTime ShowTime { get; set; } = new ShowTime();

        public IEnumerable<CinemaBranch> Branches { get; set; } = new List<CinemaBranch>();
        public IEnumerable<CinemaHall> Halls { get; set; } = new List<CinemaHall>();
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
    }
}
