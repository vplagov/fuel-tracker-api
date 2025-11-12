using FuelTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelTracker.API.Database;

public class FuelTrackerContext : DbContext
{
    public FuelTrackerContext(DbContextOptions<FuelTrackerContext> options) : base(options) {}
    
    public DbSet<CarEntity> Cars => Set<CarEntity>();
    public DbSet<FuelEntry> FuelEntries => Set<FuelEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FuelEntry>()
            .HasOne(f => f.CarEntity)
            .WithMany(c => c.FuelEntries)
            .HasForeignKey(f => f.CarId);
    }
}