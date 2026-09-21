using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;
using MovieHub.Models;

namespace MovieHub.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FavoriteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int movieId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f =>
                    f.UserId == userId.Value &&
                    f.MovieId == movieId);

            if (favorite == null)
            {
                _context.Favorites.Add(new Favorite
                {
                    UserId = userId.Value,
                    MovieId = movieId
                });
            }
            else
            {
                _context.Favorites.Remove(favorite);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Movies",
                new { id = movieId });
        }
    }
}