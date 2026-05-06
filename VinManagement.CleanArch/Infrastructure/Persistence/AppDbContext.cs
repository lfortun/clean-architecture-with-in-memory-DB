using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Vin> Vins => Set<Vin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Vin>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(17);

            entity.Property(e => e.VehicleMake)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.VehicleModel)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Code)
                .IsUnique();
        });
    }
}
