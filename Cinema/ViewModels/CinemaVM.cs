using Cinema.Models;
using Newtonsoft.Json.Linq;

namespace Cinema.ViewModels
{
    public class CinemaVM
    {
        public IEnumerable<Movie> Movies { get; set; } = new List<Movie>();
        public IEnumerable<Movie> AdultMovies { get; set; } = new List<Movie>();
        public IEnumerable<Movie> KidsMovies { get; set; } = new List<Movie>();
        public IEnumerable<Movie> NewMovies { get; set; } = new List<Movie>();
        public IEnumerable<Movie> TodayMovies { get; set; } = new List<Movie>();
        public IEnumerable<Actor> Actors { get; set; } = new List<Actor>();
    }
}
