using FuelTracker.API.Database;
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

        return new FuelEntryResponse(
            fuelEntry.Id, 
            fuelEntry.Date, 
            fuelEntry.Odometer, 
            fuelEntry.Liters,
            fuelEntry.PricePerLiter,
            fuelEntry.TotalCost);
    }
}