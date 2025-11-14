using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cinema.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<ShowTime> _showTimeRepository;
        private readonly IRepository<CinemaHall> _cinemaHallRepository;

        public HomeController(
            IRepository<Movie> movieRepository,
            IRepository<ShowTime> showTimeRepository,
            IRepository<CinemaHall> cinemaHallRepository)
        {
            _movieRepository = movieRepository;
            _showTimeRepository = showTimeRepository;
            _cinemaHallRepository = cinemaHallRepository;
        }

        public async Task<IActionResult> Index()
        {
            var movies = (await _movieRepository.GetAsync(tracked: false))
                .OrderByDescending(m => m.ReleaseDate)
                .ToList();

            var newMovies = movies.Take(6).ToList();

            var todayMovies = (await _movieRepository.GetAsync(tracked: false))
                .Where(m => m.ReleaseDate.Date == DateTime.Today)
                .ToList();

            var adultmovies = (await _movieRepository.GetAsync(tracked: false))
                .Where(m => m.AgeRestriction >= 12)
                .ToList();

            var kidsMovies = (await _movieRepository.GetAsync(tracked: false))
                .Where(m => m.AgeRestriction < 12)
                .ToList();

            var viewModel = new CinemaVM()
            {
                Movies = movies,
                NewMovies = newMovies,
                TodayMovies = todayMovies,
                AdultMovies = adultmovies,
                KidsMovies = kidsMovies
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            // جلب الفيلم
            var movie = await _movieRepository.GetOneAsync(
                expression: m => m.Id == id,
                tracked: false);

            if (movie == null)
            {
                TempData["Error"] = "Movie not found";
                return RedirectToAction("Index");
            }

            // جلب مواعيد العرض للفيلم (من اليوم وما بعده)
            var showTimes = (await _showTimeRepository.GetAsync(
                expression: s => s.MovieId == id && s.ShowDate >= DateTime.Today,
                includes:[s => s.CinemaHall ],
                tracked: false))
                .OrderBy(s => s.ShowDate)
                .ThenBy(s => s.StartTime)
                .ToList();

            var viewModel = new MovieBookingVM
            {
                Movie = movie,
                ShowTimes = showTimes,
                AvailableDates = showTimes
                    .Select(s => s.ShowDate.Date)
                    .Distinct()
                    .OrderBy(d => d)
                    .Take(7) // عرض 7 أيام فقط
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetShowTimesByDate(int movieId, DateTime date)
        {
            var showTimes = (await _showTimeRepository.GetAsync(
                expression: s => s.MovieId == movieId && s.ShowDate.Date == date.Date,
                includes: [s => s.CinemaHall],
                tracked: false))
                .OrderBy(s => s.StartTime)
                .Select(s => new
                {
                    id = s.Id,
                    cinemaHallName = s.CinemaHall.Name,
                    startTime = s.StartTime.ToString(@"hh\:mm"),
                    ticketPrice = s.TicketPrice,
                    status = s.Status
                })
                .ToList();

            return Json(showTimes);
        }

        [HttpPost]
        public IActionResult SelectShowTime(int showTimeId, int movieId)
        {
            // حفظ معلومات العرض المختار في الـ Session
            HttpContext.Session.SetInt32("SelectedShowTimeId", showTimeId);
            HttpContext.Session.SetInt32("SelectedMovieId", movieId);

            return Json(new { success = true });
        }
    }
}