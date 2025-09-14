using Microsoft.EntityFrameworkCore;

namespace Exam.Models  
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Url> Urls { get; set; }
        public DbSet<UrlClick> UrlClicks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Url>()
                .HasIndex(u => u.ShortCode)
                .IsUnique();

            modelBuilder.Entity<UrlClick>()
                .HasOne(c => c.Url)
                .WithMany()
                .HasForeignKey(c => c.UrlId);
        }
    }
}
