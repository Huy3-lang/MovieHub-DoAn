using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }

        public Movie? Movie { get; set; }
    }
}