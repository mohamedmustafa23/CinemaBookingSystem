using Cinema.Models;
using Cinema.Repositories.IRepositories;
using Cinema.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly ILogger<MovieController> _logger;
        private readonly IWebHostEnvironment _env;

        public MovieController(
            IRepository<Movie> movieRepository,
            IRepository<Category> categoryRepository,
            ILogger<MovieController> logger,
            IWebHostEnvironment env)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAsync(includes: [c => c.Category]);
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new MovieVM
            {
                Categories = await _categoryRepository.GetAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MovieVM model, IFormFile Poster, IFormFile Trailer)
        {
            model.Categories = await _categoryRepository.GetAsync();
            if (Poster == null || Poster.Length == 0)
            {
                ModelState.AddModelError("Movie.Poster", "Please upload a Poster.");
                if (Trailer == null || Trailer.Length == 0)
                {
                    ModelState.AddModelError("Movie.Trailer", "Please upload a Trailer.");
                }
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (Poster is not null && Poster.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/movies/Posters");
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var posterFileName = Guid.NewGuid().ToString() + Path.GetExtension(Poster.FileName);
                var posterFilePath = Path.Combine(path, posterFileName);
                using (var stream = new FileStream(posterFilePath, FileMode.Create))
                {
                    await Poster.CopyToAsync(stream);
                }

                model.Movie.Poster = "/images/movies/Posters/" + posterFileName;
            }


            if (Trailer != null)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos/movies/Trailers");
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var trailerFileName = Guid.NewGuid() + Path.GetExtension(Trailer.FileName);
                var trailerFilePath = Path.Combine(path, trailerFileName);

                using (var stream = new FileStream(trailerFilePath, FileMode.Create))
                {
                    await Trailer.CopyToAsync(stream);
                }

                model.Movie.Trailer = "/videos/movies/Trailers/" + trailerFileName;
            }


            await _movieRepository.AddAsync(model.Movie);
            await _movieRepository.CommitAsync();

            TempData["Notification"] = "Movie added successfully!";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();

            var model = new MovieVM
            {
                Movie = movie,
                Categories = await _categoryRepository.GetAsync()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile? Poster, IFormFile? Trailer)
        {
            
            if (!ModelState.IsValid)
            {
                var model = new MovieVM
                {
                    Movie = await _movieRepository.GetOneAsync(m => m.Id == movie.Id),
                    Categories = await _categoryRepository.GetAsync()
                };
                return View(model);
            }

            var existingMovie = await _movieRepository.GetOneAsync(m => m.Id == movie.Id);
            if (existingMovie == null)
                return NotFound();

            existingMovie.Title = movie.Title;
            existingMovie.Language = movie.Language;
            existingMovie.CategoryId = movie.CategoryId;
            existingMovie.Duration = movie.Duration;
            existingMovie.AgeRestriction = movie.AgeRestriction;
            existingMovie.ReleaseDate = movie.ReleaseDate;
            existingMovie.Subtitle = movie.Subtitle;
            existingMovie.Description = movie.Description;

            if (Poster != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "images/movies/posters");
                string fileName = Guid.NewGuid() + Path.GetExtension(Poster.FileName);
                string filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Poster.CopyToAsync(stream);

                existingMovie.Poster = "/images/movies/posters/" + fileName;

                // Delete old poster file if exists
                if (!string.IsNullOrEmpty(existingMovie.Poster))
                {
                    string oldFilePath = Path.Combine(_env.WebRootPath, existingMovie.Poster.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }


            if (Trailer != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "videos/Movies/Trailers");
                string fileName = Guid.NewGuid() + Path.GetExtension(Trailer.FileName);
                string filePath = Path.Combine(folder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await Trailer.CopyToAsync(stream);

                existingMovie.Trailer = "/videos/Movies/Trailers/" + fileName;

                // Delete old trailer file if exists
                if (!string.IsNullOrEmpty(existingMovie.Trailer))
                {
                    string oldFilePath = Path.Combine(_env.WebRootPath, existingMovie.Trailer.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }


            _movieRepository.Update(existingMovie);
            await _movieRepository.CommitAsync();

            TempData["Notification"] = "Movie updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        // ✅ Delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();

            // Delete poster file if exists
            if (!string.IsNullOrEmpty(movie.Poster))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", movie.Poster.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            // Delete trailer file if exists
            if (!string.IsNullOrEmpty(movie.Trailer))
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", movie.Trailer.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _movieRepository.Delete(movie);
            await _movieRepository.CommitAsync();

            TempData["Notification"] = "Movie deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
