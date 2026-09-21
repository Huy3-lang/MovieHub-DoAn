using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;
using MovieHub.Models;

namespace MovieHub.Controllers
{
    public class AdminEpisodesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminEpisodesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DANH SÁCH TẬP
        public async Task<IActionResult> Index(int? movieId)
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

            var query = _context.Episodes
                .Include(e => e.Movie)
                .AsQueryable();

            // Nếu chọn một bộ phim
            if (movieId.HasValue)
            {
                query = query.Where(e => e.MovieId == movieId.Value);
            }

            var episodes = await query
                .OrderBy(e => e.MovieId)
                .ThenBy(e => e.EpisodeNumber)
                .ToListAsync();

            ViewBag.MovieId = movieId;

            return View(episodes);
        }


        // TRANG THÊM TẬP
        [HttpGet]
        public async Task<IActionResult> Create(int? movieId)
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

            var movies = await _context.Movies
                .OrderBy(m => m.Title)
                .ToListAsync();

            ViewBag.Movies = movies;
            ViewBag.SelectedMovieId = movieId;

            return View();
        }


        // XỬ LÝ THÊM TẬP
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Episode episode)
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
            var duplicateEpisode = await _context.Episodes
    .AnyAsync(e =>
        e.MovieId == episode.MovieId &&
        e.EpisodeNumber == episode.EpisodeNumber);

            if (duplicateEpisode)
            {
                ViewBag.Error = "Phim này đã có số tập này.";

                var movies = await _context.Movies
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                ViewBag.Movies = movies;
                ViewBag.SelectedMovieId = episode.MovieId;

                return View(episode);
            }
            if (!ModelState.IsValid)
            {
                var movies = await _context.Movies
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                ViewBag.Movies = movies;
                ViewBag.SelectedMovieId = episode.MovieId;

                return View(episode);
            }

            _context.Episodes.Add(episode);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index),
                new { movieId = episode.MovieId });
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

            var episode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                return NotFound();
            }

            var movies = await _context.Movies
                .OrderBy(m => m.Title)
                .ToListAsync();

            ViewBag.Movies = movies;

            return View(episode);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Episode episode)
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

            if (id != episode.Id)
            {
                return NotFound();
            }

            var duplicateEpisode = await _context.Episodes
                .AnyAsync(e =>
                    e.Id != episode.Id &&
                    e.MovieId == episode.MovieId &&
                    e.EpisodeNumber == episode.EpisodeNumber);

            if (duplicateEpisode)
            {
                ViewBag.Error = "Phim này đã có số tập này.";

                var movies = await _context.Movies
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                ViewBag.Movies = movies;

                return View(episode);
            }

            if (!ModelState.IsValid)
            {
                var movies = await _context.Movies
                    .OrderBy(m => m.Title)
                    .ToListAsync();

                ViewBag.Movies = movies;

                return View(episode);
            }

            _context.Episodes.Update(episode);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index),
                new { movieId = episode.MovieId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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

            var episode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                return NotFound();
            }

            var movieId = episode.MovieId;

            _context.Episodes.Remove(episode);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Index),
                new { movieId = movieId });
        }
    }
}