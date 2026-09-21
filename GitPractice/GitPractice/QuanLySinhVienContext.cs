using
Microsoft.EntityFrameworkCore;

public class 
QuanLySinhVienContext: 
DbContext
{
    public DbSet<SinhVien> SinhViens { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseSqlServer(
            "Server=DESKTOP-BVHSNDG\\SQLEXPRESS;Database=QuanLySinhVien;Trusted_Connection=True;TrustServerCertificate=True;");
    }
      
   
}



