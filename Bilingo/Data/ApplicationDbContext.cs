using Bilingo.Models;
using Microsoft.EntityFrameworkCore;

namespace Bilingo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(x =>
            {
                x.ToTable("Users");
                x.HasKey(x => x.Id);
                x.HasIndex(x => x.Email).IsUnique();
            });
        }
    }
}
