using Microsoft.AspNetCore.Identity;
using MovieHub.Models;

namespace MovieHub.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
// =========================
// TẠO TÀI KHOẢN ADMIN
// =========================


        if (!context.Users.Any(u => u.Username == "admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@moviehub.com",
                    IsAdmin = true
                };

                var passwordHasher = new PasswordHasher<User>();

                admin.PasswordHash =
                    passwordHasher.HashPassword(admin, "Admin@123");

                context.Users.Add(admin);
                context.SaveChanges();
            }

            // =========================
            // TẠO THỂ LOẠI
            // =========================

            if (context.Categories.Any())
            {
                return;
            }

            var categories = new[]
 {
    new Category { Name = "Hành động" },
    new Category { Name = "Hài" },
    new Category { Name = "Tình cảm" },
    new Category { Name = "Hoạt hình" },
    new Category { Name = "Kinh dị" },

    new Category { Name = "Xuyên không" },
    new Category { Name = "Tu tiên" },
    new Category { Name = "Huyền huyễn" },
    new Category { Name = "Đô thị" },
    new Category { Name = "Trùng sinh" }
};
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // =========================
            // TẠO PHIM DEMO
            // =========================

            var movies = new Movie[]
            {
            new Movie
            {
                Title = "Movie Demo 01",
                Description = "Đây là bộ phim demo đầu tiên của MovieHub.",
                PosterUrl = "https://placehold.co/300x450?text=Movie+01",
                ReleaseYear = 2026,
                Duration = 120,
                CategoryId = categories[0].Id
            },

            new Movie
            {
                Title = "Movie Demo 02",
                Description = "Đây là bộ phim demo thứ hai của MovieHub.",
                PosterUrl = "https://placehold.co/300x450?text=Movie+02",
                ReleaseYear = 2026,
                Duration = 110,
                CategoryId = categories[1].Id
            },

            new Movie
            {
                Title = "Movie Demo 03",
                Description = "Đây là bộ phim demo thứ ba của MovieHub.",
                PosterUrl = "https://placehold.co/300x450?text=Movie+03",
                ReleaseYear = 2025,
                Duration = 105,
                CategoryId = categories[2].Id
            },

            new Movie
            {
                Title = "Movie Demo 04",
                Description = "Đây là bộ phim demo thứ tư của MovieHub.",
                PosterUrl = "https://placehold.co/300x450?text=Movie+04",
                ReleaseYear = 2025,
                Duration = 95,
                CategoryId = categories[3].Id
            }
            };

            context.Movies.AddRange(movies);
            context.SaveChanges();

            // =========================
            // TẠO TẬP PHIM DEMO
            // =========================

            var episodes = new Episode[]
            {
            new Episode
            {
                MovieId = movies[0].Id,
                EpisodeNumber = 1,
                Title = "Tập 1",
                VideoUrl = "https://example.com/video1.mp4"
            },

            new Episode
            {
                MovieId = movies[1].Id,
                EpisodeNumber = 1,
                Title = "Tập 1",
                VideoUrl = "https://example.com/video2.mp4"
            }
            };

            context.Episodes.AddRange(episodes);
            context.SaveChanges();
        }
    }


}
