using System.ComponentModel.DataAnnotations;

namespace MovieHub.Models
{
    public class Episode
    {
        public int Id { get; set; }

        public int MovieId { get; set; }

        public int EpisodeNumber { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        public string? VideoUrl { get; set; }

        public Movie? Movie { get; set; }
    }
}