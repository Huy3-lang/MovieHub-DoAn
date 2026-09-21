using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        [Range(1, 5)]
        public int Score { get; set; }

        public User? User { get; set; }

        public Movie? Movie { get; set; }
    }
}