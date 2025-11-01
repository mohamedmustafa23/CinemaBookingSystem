using Cinema.Models;

namespace Cinema.ViewModels
{
    public class MovieVM
    {
        public Movie Movie { get; set; } = new Movie();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
