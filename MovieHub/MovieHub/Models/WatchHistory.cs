using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class WatchHistory
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int MovieId { get; set; }

        public int EpisodeId { get; set; }

        public DateTime WatchedAt { get; set; } = DateTime.Now;

        public User? User { get; set; }

        public Movie? Movie { get; set; }

        public Episode? Episode { get; set; }
    }
}