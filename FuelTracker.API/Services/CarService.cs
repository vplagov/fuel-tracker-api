using FuelTracker.API.Database;
using FuelTracker.API.Models;
using FuelTracker.API.Repository;

namespace FuelTracker.API.Services;

public class CarService(
    CarRepository carRepository,
    FuelTrackerContext dbContext)
{
    public async Task<CarResponse> Add(CarRequest carRequest)
    {
        var carEntity = new CarEntity
        {
            Id = Guid.NewGuid(),
            Name = carRequest.Name,
        };
        carRepository.Add(carEntity);
        await dbContext.SaveChangesAsync();

        return new CarResponse(Id: carEntity.Id, Name: carEntity.Name);
    }

    public async Task<IEnumerable<CarResponse>> GetCars()
    {
        var cars = await carRepository.GetCars();
        return cars.Select(e => new CarResponse(Id: e.Id, Name: e.Name));
    }
}