using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;

namespace MovieHub.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
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

            ViewBag.MovieCount = await _context.Movies.CountAsync();
            ViewBag.UserCount = await _context.Users.CountAsync();
            ViewBag.EpisodeCount = await _context.Episodes.CountAsync();
            ViewBag.CategoryCount = await _context.Categories.CountAsync();

            return View();
        }
    }
}