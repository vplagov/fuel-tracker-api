using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Models;
using FuelTracker.API.Repositories;
using FuelTracker.API.Shared;

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

    public async Task<List<CarResponse>> GetCars()
    {
        var cars = await carRepository.GetCars();
        return cars
            .Select(e => new CarResponse(Id: e.Id, Name: e.Name))
            .ToList();
    }

    public async Task<Result<CarResponse>> GetCar(Guid id)
    {
        var car = await carRepository.GetCar(id);
        return car == null ? 
            Result<CarResponse>.Failure(ErrorType.NotFound, $"Car with id '{id}' not found") :
            Result<CarResponse>.Success(new CarResponse(car.Id, car.Name));
    }
}