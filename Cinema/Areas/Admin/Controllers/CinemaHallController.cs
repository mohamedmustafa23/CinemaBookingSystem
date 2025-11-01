using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaHallController : Controller
    {
        private readonly ILogger<CinemaHallController> _logger;
        private readonly IRepository<CinemaHall> _cinemaHallRepository;
        private readonly IRepository<CinemaBranch> _cinemaBranchRepository;

        public CinemaHallController(ILogger<CinemaHallController> logger, IRepository<CinemaHall> cinemaHallRepository, IRepository<CinemaBranch> cinemaBranchRepository)
        {
            _logger = logger;
            _cinemaHallRepository = cinemaHallRepository;
            _cinemaBranchRepository = cinemaBranchRepository;
        }

        public async Task<IActionResult> Index()
        {
            var cinemaHalls = await _cinemaHallRepository.GetAsync();
            return View(cinemaHalls);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var branches = await _cinemaBranchRepository.GetAsync();
            return View(new CinemaHallVM()
            {
                Branches = branches
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(CinemaHallVM cinemaHallVM)
        {
            if (!ModelState.IsValid)
            {
                cinemaHallVM.Branches = await _cinemaBranchRepository.GetAsync();
                return View(cinemaHallVM);
            }

            await _cinemaHallRepository.AddAsync(cinemaHallVM.CinemaHall);
            await _cinemaHallRepository.CommitAsync();
            TempData["Notification"] = "Cinema Hall created successfully!";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var hall = await _cinemaHallRepository.GetOneAsync(e => e.Id == id);
            if (hall == null) return NotFound();

            var branches = await _cinemaBranchRepository.GetAsync();

            return View(new CinemaHallVM
            {
                CinemaHall = hall,
                Branches = branches
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CinemaHallVM cinemaHallVM)
        {
            if (!ModelState.IsValid)
            {
                cinemaHallVM.Branches = await _cinemaBranchRepository.GetAsync();
                return View(cinemaHallVM);
            }
            _cinemaHallRepository.Update(cinemaHallVM.CinemaHall);
            await _cinemaHallRepository.CommitAsync();
            TempData["Notification"] = "Cinema Hall updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var hall = await _cinemaHallRepository.GetOneAsync(e => e.Id == id);
            if (hall == null) return NotFound();
            _cinemaHallRepository.Delete(hall);
            await _cinemaHallRepository.CommitAsync();
            TempData["Notification"] = "Cinema Hall deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
