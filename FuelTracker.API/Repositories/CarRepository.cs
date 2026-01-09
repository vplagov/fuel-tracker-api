using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuelTracker.API.Repositories;

public class CarRepository(FuelTrackerContext context)
{
    public void Add(CarEntity carEntity)
    {
        context.Cars.Add(carEntity);
    }

    public Task<List<CarEntity>> GetCars()
    {
        return context.Cars.ToListAsync();
    }

    public async Task<CarEntity?> GetCar(Guid id)
    {
        return await context.Cars.FindAsync(id);
    }
}