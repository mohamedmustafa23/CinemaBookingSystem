using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaBranchController : Controller
    {
        private readonly IRepository<CinemaBranch> _CinemaBranchRepository;

        public CinemaBranchController(IRepository<CinemaBranch> CinemaBranchRepository)
        {
            _CinemaBranchRepository = CinemaBranchRepository;
        }

        public async Task<IActionResult> Index()
        {
            var branches = await _CinemaBranchRepository.GetAsync();
            return View(branches);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CinemaBranch branch)
        {
            if (!ModelState.IsValid)
            {
                return View(branch);
            }

            await _CinemaBranchRepository.AddAsync(branch);
            await _CinemaBranchRepository.CommitAsync();
            TempData["Notification"] = "Branch added successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var branch = await _CinemaBranchRepository.GetOneAsync(b => b.Id == id);
            return View(branch);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CinemaBranch branch)
        {
            if (!ModelState.IsValid)
            {
                return View(branch);
            }
            _CinemaBranchRepository.Update(branch);
            await _CinemaBranchRepository.CommitAsync();
            TempData["Notification"] = "Branch updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var branch = await _CinemaBranchRepository.GetOneAsync(b => b.Id == id);
            if (branch == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            _CinemaBranchRepository.Delete(branch);
            await _CinemaBranchRepository.CommitAsync();
            TempData["Notification"] = "Branch deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
