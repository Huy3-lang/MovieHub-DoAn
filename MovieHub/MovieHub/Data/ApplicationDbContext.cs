using Microsoft.EntityFrameworkCore;
using MovieHub.Models;

namespace MovieHub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Movie> Movies { get; set; }

        public DbSet<MovieCategory> MovieCategories { get; set; }

        public DbSet<Rating> Ratings { get; set; }

        public DbSet<Episode> Episodes { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Favorite> Favorites { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<WatchHistory> WatchHistories { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // ==========================================
            // MOVIE - CATEGORY NHIỀU NHIỀU
            // ==========================================

            modelBuilder.Entity<MovieCategory>()
                .HasKey(mc => new
                {
                    mc.MovieId,
                    mc.CategoryId
                });

            modelBuilder.Entity<MovieCategory>()
      .HasOne(mc => mc.Movie)
      .WithMany(m => m.MovieCategories)
      .HasForeignKey(mc => mc.MovieId)
      .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MovieCategory>()
                .HasOne(mc => mc.Category)
                .WithMany(c => c.MovieCategories)
                .HasForeignKey(mc => mc.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // ==========================================
            // FAVORITE
            // ==========================================

            modelBuilder.Entity<Favorite>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Favorite>()
                .HasIndex(f => new
                {
                    f.UserId,
                    f.MovieId
                })
                .IsUnique();

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Movie)
                .WithMany()
                .HasForeignKey(f => f.MovieId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // RATING
            // ==========================================

            modelBuilder.Entity<Rating>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Rating>()
                .HasIndex(r => new
                {
                    r.UserId,
                    r.MovieId
                })
                .IsUnique();

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany()
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // COMMENT
            // ==========================================

            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany()
                .HasForeignKey(c => c.MovieId)
                .OnDelete(DeleteBehavior.Cascade);


            // ==========================================
            // WATCH HISTORY
            // ==========================================

            modelBuilder.Entity<WatchHistory>()
                .HasKey(w => w.Id);

            modelBuilder.Entity<WatchHistory>()
                .HasIndex(w => new
                {
                    w.UserId,
                    w.EpisodeId
                })
                .IsUnique();

            modelBuilder.Entity<WatchHistory>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WatchHistory>()
                .HasOne(w => w.Movie)
                .WithMany()
                .HasForeignKey(w => w.MovieId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<WatchHistory>()
                .HasOne(w => w.Episode)
                .WithMany()
                .HasForeignKey(w => w.EpisodeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}