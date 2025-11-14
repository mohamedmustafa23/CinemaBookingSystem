using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_SuperAdmin},{SD.Role_Admin},{SD.Role_Employee}")]
    public class ActorController : Controller
    {
        private readonly IRepository<Actor> _actorRepository;
        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }
        public async Task<IActionResult> Index()
        {
            var actor = await _actorRepository.GetAsync();
            return View(actor);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Actor());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile img)
        {
            if (img == null)
            {
                ModelState.AddModelError("Image", "Please upload an image.");
                return View(actor);
            }

            if (!ModelState.IsValid)
            {
                return View(actor);
            }
            if (img != null && img.Length > 0)
            {
                var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actors");
                if (!Directory.Exists(uploadDir))
                    Directory.CreateDirectory(uploadDir);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                actor.Image = "/images/actors/" + fileName;
            }
          
            await _actorRepository.AddAsync(actor); 
            await _actorRepository.CommitAsync();

            TempData["Notification"] = "Actor Added successfully!";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id);
            if (actor == null)
                return RedirectToAction("NotFoundPage", "Home");

            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Actor updatedActor, IFormFile? img)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id);
            if (actor == null)
                return RedirectToAction("NotFoundPage", "Home");

            if (!ModelState.IsValid)
                return View(actor);

            
            if (img != null && img.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actors");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var filePath = Path.Combine(path, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await img.CopyToAsync(stream);
                }

                // Delete old photo
                if (!string.IsNullOrEmpty(actor.Image))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", actor.Image.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                actor.Image = "/images/actors/" + fileName;
            }

            actor.Name = updatedActor.Name;
            actor.Age = updatedActor.Age;
            actor.Biography = updatedActor.Biography;

            _actorRepository.Update(actor);
            await _actorRepository.CommitAsync();

            TempData["Notification"] = "Actor updated successfully!";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{SD.Role_SuperAdmin},{SD.Role_Admin}")]
        public async Task<ActionResult> Delete(int id)
        {
            var actor = await _actorRepository.GetOneAsync(e => e.Id == id);

            if (actor == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            if (!string.IsNullOrEmpty(actor.Image))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", actor.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _actorRepository.Delete(actor);
            await _actorRepository.CommitAsync();

            TempData["Notification"] = "Actor deleted successfully!";

            return RedirectToAction(nameof(Index));
        }
    }
}
