using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Models;
using FuelTracker.API.Shared;

namespace FuelTracker.API.Services;

public class CarService(IUnitOfWork unitOfWork, UserContextService userContextService)
{
    public async Task<CarResponse> Add(CarRequest carRequest)
    {
        var userId = userContextService.GetUserId();
        var carEntity = new CarEntity
        {
            Id = Guid.NewGuid(),
            Name = carRequest.Name,
            UserId = userId,
        };
        unitOfWork.CarRepository.Add(carEntity);
        await unitOfWork.CommitAsync();

        return new CarResponse(Id: carEntity.Id, Name: carEntity.Name);
    }

    public async Task<List<CarResponse>> GetCars()
    {
        var userId = userContextService.GetUserId();
        var cars = await unitOfWork.CarRepository.GetCars(userId);
        return cars
            .Select(e => new CarResponse(Id: e.Id, Name: e.Name))
            .ToList();
    }

    public async Task<Result<CarResponse>> GetCar(Guid id)
    {
        var userId = userContextService.GetUserId();
        var car = await unitOfWork.CarRepository.GetCar(id, userId);
        return car == null  ? 
            Result<CarResponse>.Failure(ErrorType.NotFound, $"Car with id '{id}' not found") :
            Result<CarResponse>.Success(new CarResponse(car.Id, car.Name));
    }
}