using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieHub.Data;

namespace MovieHub.Controllers
{
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminUsersController(ApplicationDbContext context)
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

            // Lấy danh sách người dùng
            var users = await _context.Users
                .OrderBy(u => u.Id)
                .ToListAsync();

            return View(users);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(int id)
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

            // Tìm người dùng
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Không cho tự hủy quyền Admin của chính mình
            if (user.Username == username)
            {
                TempData["Error"] = "Bạn không thể thay đổi quyền của chính mình.";
                return RedirectToAction(nameof(Index));
            }

            // Đổi quyền
            user.IsAdmin = !user.IsAdmin;

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

            // Tìm người dùng
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Không cho tự xóa tài khoản đang đăng nhập
            if (user.Username == username)
            {
                TempData["Error"] = "Bạn không thể tự xóa tài khoản đang đăng nhập.";
                return RedirectToAction(nameof(Index));
            }

            // Xóa người dùng
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}