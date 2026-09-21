using System.ComponentModel.DataAnnotations;

public class SinhVien
{
    [Key]
    public string RollNo { get; set; }

    public string FullName { get; set; }

    public int Age { get; set; }
}