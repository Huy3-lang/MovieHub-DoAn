using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Quan hệ cũ
        public ICollection<Movie> Movies { get; set; }
            = new List<Movie>();

        // Quan hệ nhiều-nhiều mới
        public ICollection<MovieCategory> MovieCategories { get; set; }
            = new List<MovieCategory>();
    }
}