using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;
using MovieHub.Models;

namespace MovieHub.Controllers
{
    public class RatingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int movieId, int score)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (score < 1 || score > 5)
            {
                return RedirectToAction(
                    "Details",
                    "Movies",
                    new { id = movieId });
            }

            var movieExists = await _context.Movies
                .AnyAsync(m => m.Id == movieId);

            if (!movieExists)
            {
                return NotFound();
            }

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r =>
                    r.UserId == userId.Value &&
                    r.MovieId == movieId);

            if (rating == null)
            {
                rating = new Rating
                {
                    UserId = userId.Value,
                    MovieId = movieId,
                    Score = score
                };

                _context.Ratings.Add(rating);
            }
            else
            {
                rating.Score = score;
                _context.Ratings.Update(rating);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Movies",
                new { id = movieId });
        }
    }
}