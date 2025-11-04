using FuelTracker.API.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelTracker.API.Database;

public class FuelTrackerContext : DbContext
{
    public FuelTrackerContext(DbContextOptions<FuelTrackerContext> options) : base(options) {}
    
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<FuelEntry> FuelEntries => Set<FuelEntry>();
}