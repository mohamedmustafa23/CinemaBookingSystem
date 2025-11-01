using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ShowtimeController : Controller
    {
        private readonly ILogger<ShowtimeController> _logger;
        private readonly IRepository<ShowTime> _showtimeRepository;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<CinemaBranch> _branchRepository;
        private readonly IRepository<CinemaHall> _hallRepository;

        public ShowtimeController(ILogger<ShowtimeController> logger, IRepository<ShowTime> showtimeRepository, IRepository<CinemaBranch> branchRepository, IRepository<CinemaHall> hallRepository, IRepository<Movie> movieRepository)
        {
            _logger = logger;
            _showtimeRepository = showtimeRepository;
            _branchRepository = branchRepository;
            _hallRepository = hallRepository;
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> Index()
        {
            var showtimes = await _showtimeRepository.GetAsync
                (
                    includes:
                    [
                        s => s.Movie,
                        s => s.CinemaHall,
                        s => s.CinemaHall.Branch
                    ]
                );

            return View(showtimes);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var branches = await _branchRepository.GetAsync(tracked: false);
            var halls = await _hallRepository.GetAsync(tracked: false);
            var movies = await _movieRepository.GetAsync(tracked: false);
            return View(new ShowTimeVM()
            {
                Branches = branches,
                Halls = halls,
                Movies = movies
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetHallsByBranch(int branchId)
        {
            var halls = await _hallRepository.GetAsync(h => h.BranchId == branchId, tracked: false);
            var result = halls.Select(h => new { id = h.Id, name = h.Name });
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ShowTime showtime)
        {
            if (!ModelState.IsValid)
            {
                var branches = await _branchRepository.GetAsync(tracked: false);
                var halls = await _hallRepository.GetAsync(tracked: false);
                var movies = await _movieRepository.GetAsync(tracked: false);
                var showtimeVM = new ShowTimeVM()
                {
                    ShowTime = showtime,
                    Branches = branches,
                    Halls = halls,
                    Movies = movies
                };
                return View(showtimeVM);
            }


            await _showtimeRepository.AddAsync(showtime);
            await _showtimeRepository.CommitAsync();

            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var showtime = await _showtimeRepository.GetOneAsync(
                s => s.Id == id,
                includes: [s => s.Movie, s => s.CinemaHall, s => s.CinemaHall.Branch]
            );

            if (showtime == null)
                return NotFound();

            var movies = await _movieRepository.GetAsync(tracked: false);
            var halls = await _hallRepository.GetAsync(tracked: false);
            var branches = await _branchRepository.GetAsync(tracked: false);

            var viewModel = new ShowTimeVM
            {
                ShowTime = showtime,
                Movies = movies,
                Halls = halls,
                Branches = branches
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ShowTime showtime)
        {
            if (!ModelState.IsValid)
            {
                var movies = await _movieRepository.GetAsync(tracked: false);
                var halls = await _hallRepository.GetAsync(tracked: false);
                var branches = await _branchRepository.GetAsync(tracked: false);

                var vm = new ShowTimeVM
                {
                    ShowTime = showtime,
                    Movies = movies,
                    Halls = halls,
                    Branches = branches
                };

                return View(vm);
            }

            _showtimeRepository.Update(showtime);
            await _showtimeRepository.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var showtime = await _showtimeRepository.GetOneAsync(s => s.Id == id);
            if (showtime == null)
                return NotFound();
            _showtimeRepository.Delete(showtime);
            await _showtimeRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
