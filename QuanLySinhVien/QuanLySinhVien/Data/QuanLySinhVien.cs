using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Data;

public class QuanLySinhVienContext : DbContext
{
    public QuanLySinhVienContext(DbContextOptions<QuanLySinhVienContext> options)
        : base(options)
    {
    }

    public DbSet<SinhVien> SinhViens { get; set; }
}