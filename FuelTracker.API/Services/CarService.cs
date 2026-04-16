using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Extensions;
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
        
        return carEntity.ToResponse();
    }

    public async Task<List<CarResponse>> GetCars()
    {
        var userId = userContextService.GetUserId();
        var cars = await unitOfWork.CarRepository.GetCars(userId);
        return cars
            .Select(e => e.ToResponse())
            .ToList();
    }

    public async Task<Result<CarResponse>> GetCar(Guid id)
    {
        var userId = userContextService.GetUserId();
        var car = await unitOfWork.CarRepository.GetCar(id, userId);
        return car == null  ? 
            Result<CarResponse>.Failure(ErrorType.NotFound, $"Car with id '{id}' not found") :
            Result<CarResponse>.Success(car.ToResponse());
    }
}