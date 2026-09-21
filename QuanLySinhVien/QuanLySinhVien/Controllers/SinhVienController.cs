using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Data;

namespace QuanLySinhVien.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SinhVienController : ControllerBase
{
    private readonly QuanLySinhVienContext _context;

    public SinhVienController(QuanLySinhVienContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var danhSach = _context.SinhViens.ToList();

        return Ok(danhSach);
    }
}