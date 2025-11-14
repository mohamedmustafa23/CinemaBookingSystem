using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_SuperAdmin},{SD.Role_Admin},{SD.Role_Employee}")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<ShowTime> _showtimeRepository;
        private readonly IRepository<CinemaHall> _hallRepository;

        public DashboardController(
            ILogger<DashboardController> logger,
            IRepository<Actor> actorRepository,
            IRepository<Movie> movieRepository,
            IRepository<ShowTime> showtimeRepository,
            IRepository<CinemaHall> hallRepository)
        {
            _logger = logger;
            _actorRepository = actorRepository;
            _movieRepository = movieRepository;
            _showtimeRepository = showtimeRepository;
            _hallRepository = hallRepository;
        }

        public async Task<IActionResult> Index()
        {
            var actors = await _actorRepository.GetAsync();
            var movies = await _movieRepository.GetAsync();
            var halls = await _hallRepository.GetAsync();
            var showtimes = await _showtimeRepository.GetAsync();

            var model = new DashboardViewModel
            {
                MoviesCount = movies.Count(),
                HallsCount = halls.Count(),
                ActorsCount = actors.Count(),
                ShowtimesCount = showtimes.Count(),
                TodayShowtimesCount = showtimes.Count(s => s.ShowDate.Date == DateTime.Today),
            };

            return View(model);
        }
    }
}
