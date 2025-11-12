using FuelTracker.API.Database;
using FuelTracker.API.Models;

namespace FuelTracker.API.Repositories;

public class FuelEntryRepository(FuelTrackerContext context)
{
    public void Add(FuelEntry fuelEntry)
    {
        context.FuelEntries.Add(fuelEntry);
    }
}