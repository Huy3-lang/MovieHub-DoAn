using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;
using MovieHub.Models;

namespace MovieHub.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Episodes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                ViewBag.IsFavorite = await _context.Favorites
                    .AnyAsync(f =>
                        f.UserId == userId.Value &&
                        f.MovieId == id);
            }
            else
            {
                ViewBag.IsFavorite = false;
            }
            var ratings = await _context.Ratings
                .Where(r => r.MovieId == id)
                .ToListAsync();

            ViewBag.RatingCount = ratings.Count;

            ViewBag.AverageRating = ratings.Any()
                ? ratings.Average(r => r.Score)
                : 0;

            if (userId != null)
            {
                var userRating = await _context.Ratings
                    .FirstOrDefaultAsync(r =>
                        r.UserId == userId.Value &&
                        r.MovieId == id);

                ViewBag.UserRating = userRating?.Score ?? 0;
            }
            else
            {
                ViewBag.UserRating = 0;
            }
            var comments = await _context.Comments
    .Include(c => c.User)
    .Where(c => c.MovieId == id)
    .OrderByDescending(c => c.CreatedAt)
    .ToListAsync();

            ViewBag.Comments = comments;
            return View(movie);
        }
        public async Task<IActionResult> Watch(int id)
        {
            var episode = await _context.Episodes
                .Include(e => e.Movie)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (episode == null)
            {
                return NotFound();
            }
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId != null)
            {
                var history = await _context.WatchHistories
                    .FirstOrDefaultAsync(w =>
                        w.UserId == userId.Value &&
                        w.EpisodeId == episode.Id);

                if (history == null)
                {
                    history = new WatchHistory
                    {
                        UserId = userId.Value,
                        MovieId = episode.MovieId,
                        EpisodeId = episode.Id,
                        WatchedAt = DateTime.Now
                    };

                    _context.WatchHistories.Add(history);
                }
                else
                {
                    history.WatchedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
            }
            var episodes = await _context.Episodes
                .Where(e => e.MovieId == episode.MovieId)
                .OrderBy(e => e.EpisodeNumber)
                .ToListAsync();

            if (episode.Movie != null)
            {
                episode.Movie.Episodes = episodes;
            }

            var currentIndex = episodes.FindIndex(e => e.Id == episode.Id);

            if (currentIndex > 0)
            {
                ViewBag.PreviousEpisodeId = episodes[currentIndex - 1].Id;
            }

            if (currentIndex < episodes.Count - 1)
            {
                ViewBag.NextEpisodeId = episodes[currentIndex + 1].Id;
            }

            return View(episode);
        }
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View(new List<Movie>());
            }

            var movies = await _context.Movies
                .Include(m => m.Category)
                .Where(m => m.Title.Contains(keyword))
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            var ratings = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    Average = g.Average(r => r.Score),
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.Ratings = ratings;

            return View(movies);
        }

        public async Task<IActionResult> Category(int id, int? year)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var query = _context.Movies
                .Include(m => m.Category)
                .Where(m => m.CategoryId == id);

            if (year.HasValue)
            {
                query = query
                    .Where(m => m.ReleaseYear == year.Value);
            }

            var movies = await query
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            var years = await _context.Movies
                .Where(m => m.CategoryId == id)
                .Select(m => m.ReleaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var ratings = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    Average = g.Average(r => r.Score),
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;
            ViewBag.SelectedYear = year;
            ViewBag.Years = years;
            ViewBag.Ratings = ratings;

            return View(movies);
        }
        public async Task<IActionResult> SingleMovies(int? year)
        {
            var query = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Episodes)
                .Where(m => m.Episodes.Count == 1);

            if (year.HasValue)
            {
                query = query
                    .Where(m => m.ReleaseYear == year.Value);
            }

            var movies = await query
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            var years = await _context.Movies
                .Where(m => m.Episodes.Count == 1)
                .Select(m => m.ReleaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var ratings = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    Average = g.Average(r => r.Score),
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.Ratings = ratings;
            ViewBag.Years = years;
            ViewBag.SelectedYear = year;

            ViewData["Title"] = "Phim lẻ";

            return View("MovieList", movies);
        }


        public async Task<IActionResult> SeriesMovies(int? year)
        {
            var query = _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Episodes)
                .Where(m => m.Episodes.Count >= 2);

            if (year.HasValue)
            {
                query = query
                    .Where(m => m.ReleaseYear == year.Value);
            }

            var movies = await query
                .OrderByDescending(m => m.Id)
                .ToListAsync();

            var years = await _context.Movies
                .Where(m => m.Episodes.Count >= 2)
                .Select(m => m.ReleaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var ratings = await _context.Ratings
                .GroupBy(r => r.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    Average = g.Average(r => r.Score),
                    Count = g.Count()
                })
                .ToListAsync();

            ViewBag.Ratings = ratings;
            ViewBag.Years = years;
            ViewBag.SelectedYear = year;

            ViewData["Title"] = "Phim bộ";

            return View("MovieList", movies);
        }
    }
}