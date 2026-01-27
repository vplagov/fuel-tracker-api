using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Extensions;
using FuelTracker.API.Models;
using FuelTracker.API.Shared;

namespace FuelTracker.API.Services;

public class FuelEntryService(IUnitOfWork unitOfWork)
{
    public async Task<Result<FuelEntryResponse>> Add(Guid carId, FuelEntryRequest request)
    {
        var car = await unitOfWork.CarRepository.GetCar(carId);
        if (car == null)
        {
            return Result<FuelEntryResponse>.Failure(ErrorType.NotFound, $"Car with ID '{carId}' not found");
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
        
        unitOfWork.FuelEntryRepository.Add(fuelEntry);
        await unitOfWork.CommitAsync();

        return Result<FuelEntryResponse>.Success(fuelEntry.ToResponse());
    }

    public async Task<Result<List<FuelEntryResponse>>> GetFuelEntries(Guid carId)
    {
        var car = await unitOfWork.CarRepository.GetCar(carId);
        if (car == null)
        {
            return  Result<List<FuelEntryResponse>>
                .Failure(ErrorType.NotFound, $"Car with ID '{carId}' not found");
        }
        
        var fuelEntries = await unitOfWork.FuelEntryRepository.GetFuelEntries(carId);
        var fuelEntryResponses = fuelEntries
            .Select(fuelEntry => fuelEntry.ToResponse())
            .ToList();
        return Result<List<FuelEntryResponse>>
            .Success(fuelEntryResponses);
    }

    public async Task<Result> Remove(Guid fuelEntryId)
    {
        var fuelEntryEntity = await unitOfWork.FuelEntryRepository.GetFuelEntry(fuelEntryId);
        if (fuelEntryEntity == null)
        {
            return Result.Failure(ErrorType.NotFound, $"Fuel entry with ID '{fuelEntryId}' is not found");
        }
        
        unitOfWork.FuelEntryRepository.Remove(fuelEntryEntity);
        await unitOfWork.CommitAsync();

        return Result.Success();
    }

    public async Task<Result<FuelEntryResponse>> Update(Guid fuelEntryId, FuelEntryRequest payload)
    {
        var fuelEntryEntity = await unitOfWork.FuelEntryRepository.GetFuelEntry(fuelEntryId);
        if (fuelEntryEntity == null)
        {
            return Result<FuelEntryResponse>
                .Failure(ErrorType.NotFound, $"Fuel entry with ID '{fuelEntryId}' is not found");
        }

        fuelEntryEntity.Date = payload.Date;
        fuelEntryEntity.Odometer = payload.Odometer;
        fuelEntryEntity.Liters = payload.Liters;
        fuelEntryEntity.PricePerLiter = payload.PricePerLiter;
        fuelEntryEntity.TotalCost = payload.Liters * payload.PricePerLiter;
        
        await unitOfWork.CommitAsync();

        return Result<FuelEntryResponse>.Success(fuelEntryEntity.ToResponse());
    }
}