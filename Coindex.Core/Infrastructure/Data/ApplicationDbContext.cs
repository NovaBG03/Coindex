using Coindex.Core.Domain.Entities;
using Coindex.Core.Infrastructure.Interceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coindex.Core.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<CollectableItem> CollectableItems { get; set; } = null!;
    public DbSet<Coin> Coins { get; set; } = null!;
    public DbSet<Bill> Bills { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.AddInterceptors(new TimestampInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<BaseEntity>();

        modelBuilder.Entity<CollectableItem>(collectableItem =>
        {
            ConfigureBaseEntity(collectableItem);
            collectableItem.UseTptMappingStrategy();

            collectableItem.Property(ci => ci.Name)
                .IsRequired()
                .HasMaxLength(100);

            collectableItem.Property(c => c.Description)
                .HasMaxLength(500);

            collectableItem.HasMany(ci => ci.Tags)
                .WithMany(t => t.CollectableItems);
        });

        // modelBuilder.Entity<Bill>(bill => { });
        // modelBuilder.Entity<Coin>(coin => { });

        modelBuilder.Entity<Tag>(tag =>
        {
            ConfigureBaseEntity(tag);
            tag.HasIndex(t => t.Name)
                .IsUnique();
        });
    }

    private static void ConfigureBaseEntity<T>(EntityTypeBuilder<T> baseEntity) where T : BaseEntity
    {
        baseEntity.HasKey(e => e.Id);
    }
}
