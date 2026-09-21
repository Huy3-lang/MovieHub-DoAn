using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? PosterUrl { get; set; }

        public int ReleaseYear { get; set; }

        public int Duration { get; set; }

        // Giữ lại CategoryId cũ để không làm hỏng database hiện tại
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<Episode> Episodes { get; set; }
            = new List<Episode>();

        // Một phim có thể có nhiều thể loại
        public ICollection<MovieCategory> MovieCategories { get; set; }
            = new List<MovieCategory>();
    }
}