using Coindex.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coindex.Core.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Coin> Coins { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Coin>(coin =>
        {
            coin.HasKey(c => c.Id);

            coin.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            coin.Property(c => c.Description)
                .HasMaxLength(500);
        });
    }
}
