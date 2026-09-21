using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;
using MovieHub.Models;
using Microsoft.AspNetCore.Http;

namespace MovieHub.Controllers
{
    public class AdminMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra quyền Admin
            var isAdmin = HttpContext.Session.GetInt32("IsAdmin");

            if (isAdmin != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách phim + thể loại
            var movies = await _context.Movies
                .Include(m => m.Category)
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            return View(movies);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // Kiểm tra đăng nhập
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra quyền Admin
            var isAdmin = HttpContext.Session.GetInt32("IsAdmin");

            if (isAdmin != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    Movie movie,
    IFormFile? posterFile)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var isAdmin = HttpContext.Session.GetInt32("IsAdmin");

            if (isAdmin != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View(movie);
            }

            // Nếu có chọn ảnh từ máy
            if (posterFile != null && posterFile.Length > 0)
            {
                var allowedExtensions = new[]
                {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                var extension = Path.GetExtension(posterFile.FileName)
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(
                        "posterFile",
                        "Chỉ được chọn JPG, JPEG, PNG hoặc WEBP.");

                    ViewBag.Categories = await _context.Categories
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    return View(movie);
                }

                if (posterFile.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "posterFile",
                        "Ảnh không được vượt quá 5MB.");

                    ViewBag.Categories = await _context.Categories
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    return View(movie);
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "movies");

                Directory.CreateDirectory(uploadsFolder);

                var fileName =
                    $"{Guid.NewGuid():N}{extension}";

                var filePath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                movie.PosterUrl = $"/images/movies/{fileName}";
            }

            _context.Movies.Add(movie);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            // Kiểm tra đăng nhập
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kiểm tra quyền Admin
            var isAdmin = HttpContext.Session.GetInt32("IsAdmin");

            if (isAdmin != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = categories;

            return View(movie);
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
    int id,
    Movie movie,
    IFormFile? posterFile,
    int[]? categoryIds)
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account");
            }

            var isAdmin = HttpContext.Session.GetInt32("IsAdmin");

            if (isAdmin != 1)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id != movie.Id)
            {
                return NotFound();
            }

            var existingMovie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existingMovie == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View(movie);
            }

            // Upload ảnh mới
            if (posterFile != null && posterFile.Length > 0)
            {
                var allowedExtensions = new[]
                {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

                var extension = Path.GetExtension(posterFile.FileName)
                    .ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ViewBag.Error = "Chỉ được chọn JPG, JPEG, PNG hoặc WEBP.";

                    ViewBag.Categories = await _context.Categories
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    return View(movie);
                }

                if (posterFile.Length > 5 * 1024 * 1024)
                {
                    ViewBag.Error = "Ảnh không được vượt quá 5MB.";

                    ViewBag.Categories = await _context.Categories
                        .OrderBy(c => c.Name)
                        .ToListAsync();

                    return View(movie);
                }

                var uploadsFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    "movies");

                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid():N}{extension}";

                var filePath = Path.Combine(
                    uploadsFolder,
                    fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await posterFile.CopyToAsync(stream);
                }

                existingMovie.PosterUrl =
                    $"/images/movies/{fileName}";
            }
            else
            {
                // Nếu không chọn ảnh mới thì giữ ảnh cũ.
                // Nếu người dùng nhập Link Poster mới thì dùng link đó.
                if (!string.IsNullOrWhiteSpace(movie.PosterUrl))
                {
                    existingMovie.PosterUrl = movie.PosterUrl;
                }
            }


            existingMovie.Title = movie.Title;
            existingMovie.Description = movie.Description;
            existingMovie.ReleaseYear = movie.ReleaseYear;
            existingMovie.Duration = movie.Duration;

            // Lấy danh sách thể loại đã chọn
            var selectedCategoryIds = (categoryIds ?? Array.Empty<int>())
                .Distinct()
                .ToList();

            if (!selectedCategoryIds.Any())
            {
                ViewBag.Error = "Vui lòng chọn ít nhất một thể loại.";

                ViewBag.Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return View(movie);
            }

            // Giữ CategoryId cũ để tương thích database
            existingMovie.CategoryId = selectedCategoryIds.First();

            await _context.SaveChangesAsync();

            // Xóa các thể loại cũ
            var oldCategories = await _context.MovieCategories
                .Where(mc => mc.MovieId == existingMovie.Id)
                .ToListAsync();

            _context.MovieCategories.RemoveRange(oldCategories);

            // Thêm các thể loại mới
            foreach (var categoryId in selectedCategoryIds)
            {
                _context.MovieCategories.Add(new MovieCategory
                {
                    MovieId = existingMovie.Id,
                    CategoryId = categoryId
                });
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    // Kiểm tra đăng nhập
    var username = HttpContext.Session.GetString("Username");

    if (string.IsNullOrEmpty(username))
    {
        return RedirectToAction("Login", "Account");
    }

    // Kiểm tra quyền Admin
    var isAdmin = HttpContext.Session.GetInt32("IsAdmin");

    if (isAdmin != 1)
    {
        return RedirectToAction("Index", "Home");
    }

    var movie = await _context.Movies
        .Include(m => m.Episodes)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (movie == null)
    {
        return NotFound();
    }

    // Xóa các tập phim trước
    if (movie.Episodes.Any())
    {
        _context.Episodes.RemoveRange(movie.Episodes);
    }

    // Xóa phim
    _context.Movies.Remove(movie);

    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}
    }
}