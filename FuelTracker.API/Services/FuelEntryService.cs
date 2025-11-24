using FuelTracker.API.Database;
using FuelTracker.API.Extensions;
using FuelTracker.API.Models;
using FuelTracker.API.Repositories;

namespace FuelTracker.API.Services;

public class FuelEntryService(
    FuelEntryRepository fuelEntryRepository,
    CarRepository carRepository,
    FuelTrackerContext context)
{
    public async Task<FuelEntryResponse?> Add(Guid carId, FuelEntryRequest request)
    {
        var car = await carRepository.GetCar(carId);
        if (car == null)
        {
            return null;
        }

        var fuelEntry = new FuelEntry
        {
            CarId = carId,
            Date = request.Date,
            Id = Guid.NewGuid(),
            Liters = request.Liters,
            Odometer = request.Odometer,
            PricePerLiter = request.PricePerLiter,
            TotalCost = request.PricePerLiter * request.Liters
        };
        
        fuelEntryRepository.Add(fuelEntry);
        await context.SaveChangesAsync();

        return fuelEntry.ToResponse();
    }

    public async Task<IList<FuelEntryResponse>?> GetFuelEntries(Guid carId)
    {
        var car = await carRepository.GetCar(carId);
        if (car == null)
        {
            return null;
        }
        
        var fuelEntries = await fuelEntryRepository.GetFuelEntries(carId);
        return fuelEntries
            .Select(fuelEntry => fuelEntry.ToResponse())
            .ToList();
    }

    public async Task<bool> Remove(Guid fuelEntryId)
    {
        var fuelEntryEntity = await fuelEntryRepository.GetFuelEntry(fuelEntryId);
        if (fuelEntryEntity == null)
        {
            return false;
        }
        
        fuelEntryRepository.Remove(fuelEntryEntity);
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<FuelEntryResponse?> Update(Guid fuelEntryId, FuelEntryRequest payload)
    {
        var fuelEntryEntity = await fuelEntryRepository.GetFuelEntry(fuelEntryId);
        if (fuelEntryEntity == null)
        {
            return null;
        }

        fuelEntryEntity.Date = payload.Date;
        fuelEntryEntity.Odometer = payload.Odometer;
        fuelEntryEntity.Liters = payload.Liters;
        fuelEntryEntity.PricePerLiter = payload.PricePerLiter;
        fuelEntryEntity.TotalCost = payload.Liters * payload.PricePerLiter;
        
        await context.SaveChangesAsync();

        return fuelEntryEntity.ToResponse();
    }
}