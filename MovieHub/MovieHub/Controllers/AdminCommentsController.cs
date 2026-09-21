using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;

namespace MovieHub.Controllers
{
    public class AdminCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
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

            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Movie)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(comments);
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

            var comment = await _context.Comments
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}