using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuelTracker.API.Repositories;

public class FuelEntryRepository(FuelTrackerContext context)
{
    public void Add(FuelEntry fuelEntry)
    {
        context.FuelEntries.Add(fuelEntry);
    }

    public Task<List<FuelEntry>> GetFuelEntries(Guid carId)
    {
        return context.FuelEntries
            .Where(entry => entry.CarId == carId)
            .OrderByDescending(entry => entry.Date)
            .ThenByDescending(entry => entry.Id)
            .ToListAsync();
    }

    public Task<List<FuelEntry>> GetFuelEntries(Guid carId, int page, int pageSize)
    {
        return context.FuelEntries
            .Where(entry => entry.CarId == carId)
            .OrderByDescending(entry => entry.Date)
            .ThenByDescending(entry => entry.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<int> CountFuelEntries(Guid carId)
    {
        return context.FuelEntries
            .Where(entry => entry.CarId == carId)
            .CountAsync();
    }

    public Task<FuelEntry?> GetFuelEntry(Guid fuelEntryId, Guid userId)
    {
        return context.FuelEntries
            .FirstOrDefaultAsync(f => f.Id == fuelEntryId && f.CarEntity.UserId == userId);
    }

    public void Remove(FuelEntry fuelEntry)
    {
        context.FuelEntries.Remove(fuelEntry);
    }
}