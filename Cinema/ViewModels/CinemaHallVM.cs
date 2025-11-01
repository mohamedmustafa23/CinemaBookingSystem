using Cinema.Models;
namespace Cinema.ViewModels
{
    public class CinemaHallVM
    {
        public CinemaHall CinemaHall { get; set; } = new CinemaHall();
        public IEnumerable<CinemaBranch> Branches { get; set; } = new List<CinemaBranch>();
    }
}
