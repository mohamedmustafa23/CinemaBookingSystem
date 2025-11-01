using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;

        public HomeController(IRepository<Movie> movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IActionResult> Index()
        {
            // هتجيب آخر 8 أفلام شعبية
            var popularMovies = (await _movieRepository.GetAsync(tracked: false))
                .OrderByDescending(m => m.Rate)
                .Take(8)
                .ToList();

            // و آخر 6 أفلام تم إضافتهم (الإصدارات الجديدة)
            var newMovies = (await _movieRepository.GetAsync(tracked: false))
                .OrderByDescending(m => m.ReleaseDate)
                .Take(6)
                .ToList();
            var todayMovies = (await _movieRepository.GetAsync(tracked: false))
                .Where(m => m.ReleaseDate == DateTime.Today)
                .ToList();
            var movies = (await _movieRepository.GetAsync(tracked: false))
                .OrderByDescending(m => m.ReleaseDate)
                .ToList();

            var viewModel = new CinemaVM()
            {
                Movies = movies,
                NewMovies = newMovies,
                TodayMovies = todayMovies
            };

            return View(viewModel);
        }
    }
}
