using Microsoft.EntityFrameworkCore;
using var db= new QuanLySinhVienContext();
var danhSach = db.SinhViens.ToList();
foreach (var sv in danhSach)
{
    Console.WriteLine($"{sv.RollNo}-{sv.FullName}-{sv.Age}");

}