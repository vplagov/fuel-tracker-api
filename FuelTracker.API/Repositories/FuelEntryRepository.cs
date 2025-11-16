using FuelTracker.API.Database;
using FuelTracker.API.Models;
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
            .ToListAsync();
    }

    public ValueTask<FuelEntry?> GetFuelEntry(Guid fuelEntryId)
    {
        return context.FuelEntries.FindAsync(fuelEntryId);
    }

    public void Remove(FuelEntry fuelEntry)
    {
        context.FuelEntries.Remove(fuelEntry);
    }
}