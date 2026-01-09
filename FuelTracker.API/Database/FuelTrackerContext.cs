using FuelTracker.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuelTracker.API.Database;

public class FuelTrackerContext : DbContext
{
    public FuelTrackerContext(DbContextOptions<FuelTrackerContext> options) : base(options)
    {
    }

    public DbSet<CarEntity> Cars => Set<CarEntity>();
    public DbSet<FuelEntry> FuelEntries => Set<FuelEntry>();
    public DbSet<UserEntity> Users => Set<UserEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FuelEntry>()
            .HasOne(f => f.CarEntity)
            .WithMany(c => c.FuelEntries)
            .HasForeignKey(f => f.CarId);

        modelBuilder.Entity<UserEntity>(userEntity =>
        {
            userEntity.HasIndex(u => u.Username).IsUnique();
            userEntity.Property(u => u.Email).IsRequired(false);
            userEntity.Property(u => u.CreatedAt).IsRequired(false);
        });
        
        modelBuilder.Entity<CarEntity>()
            .HasOne(carEntity => carEntity.UserEntity)
            .WithMany(userEntity => userEntity.CarEntities)
            .HasForeignKey(carEntity => carEntity.UserId);
    }
}