using Microsoft.EntityFrameworkCore;
using WebApi.Database.Entities;

namespace WebApi.Database
{
    public class WebApiDbContext : DbContext
    {
        public WebApiDbContext(DbContextOptions<WebApiDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bet> Bets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Balance).IsRequired();
            });

            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(e => e.BetId);
                entity.Property(e => e.BetAmount).IsRequired();
                entity.Property(e => e.BetTime).IsRequired();
                entity.Property(e => e.Details).HasMaxLength(255);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.Bets)
                      .HasForeignKey(d => d.UserId)
                      .OnDelete(DeleteBehavior.ClientSetNull);
            });
        }
    }
}
