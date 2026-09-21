using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;

namespace MovieHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 8;

            if (page < 1)
            {
                page = 1;
            }

            var totalMovies = await _context.Movies.CountAsync();

            var totalPages = (int)Math.Ceiling(
                totalMovies / (double)pageSize);

            if (totalPages > 0 && page > totalPages)
            {
                page = totalPages;
            }

            var movies = await _context.Movies
                .Include(m => m.Category)
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(movies);
        }
    }
}