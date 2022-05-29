using Bilingo.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilingo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<UserWord> UserWords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(x =>
            {
                x.ToTable("Users");
                x.HasKey(x => x.Id);
                x.HasIndex(x => x.Email).IsUnique();
                x.HasMany(x => x.Words)
                    .WithMany(x => x.Users)
                    .UsingEntity<UserWord>(
                        y => y
                            .HasOne(y => y.Word)
                            .WithMany(y => y.UserWords)
                            .HasForeignKey(y => y.WordId),
                        y => y
                            .HasOne(y => y.User)
                            .WithMany(y => y.UserWords)
                            .HasForeignKey(y => y.UserId),
                        y =>
                        {
                            y.HasKey(y => new { y.WordId, y.UserId });
                        });
            });

            builder.Entity<Word>(x =>
            {
                x.ToTable("Words");
                x.HasKey(x => x.Id);
                x.HasIndex(x => x.Value).IsUnique();
            });
        }
    }
}
