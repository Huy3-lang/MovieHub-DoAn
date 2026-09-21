using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;
using MovieHub.Models;

namespace MovieHub.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int movieId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction(
                    "Details",
                    "Movies",
                    new { id = movieId });
            }

            if (content.Length > 1000)
            {
                content = content.Substring(0, 1000);
            }

            var movieExists = await _context.Movies
                .AnyAsync(m => m.Id == movieId);

            if (!movieExists)
            {
                return NotFound();
            }

            var comment = new Comment
            {
                UserId = userId.Value,
                MovieId = movieId,
                Content = content.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "Details",
                "Movies",
                new { id = movieId });
        }
    }
}