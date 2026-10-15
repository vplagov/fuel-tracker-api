using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Extensions;
using FuelTracker.API.Models;
using FuelTracker.API.Shared;

namespace FuelTracker.API.Services;

public class FuelEntryService(IUnitOfWork unitOfWork, UserContextService userContextService)
{
    private const int MaxPageSize = 100;

    public async Task<Result<FuelEntryResponse>> Add(Guid carId, FuelEntryRequest request)
    {
        var userId = userContextService.GetUserId();
        var car = await unitOfWork.CarRepository.GetCar(carId, userId);
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
            TotalCost = request.PricePerLiter * request.Liters,
            IsFullTank = request.IsFullTank
        };
        
        unitOfWork.FuelEntryRepository.Add(fuelEntry);
        await unitOfWork.CommitAsync();

        return Result<FuelEntryResponse>.Success(fuelEntry.ToResponse());
    }

    public async Task<Result<PagedResponse<FuelEntryResponse>>> GetFuelEntries(
        Guid carId, int page, int pageSize)
    {
        var userId = userContextService.GetUserId();
        var car = await unitOfWork.CarRepository.GetCar(carId, userId);
        if (car == null)
        {
            return Result<PagedResponse<FuelEntryResponse>>
                .Failure(ErrorType.NotFound, $"Car with ID '{carId}' not found");
        }

        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var totalCount = await unitOfWork.FuelEntryRepository.CountFuelEntries(carId);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var fuelEntries = await unitOfWork.FuelEntryRepository
            .GetFuelEntries(carId, page, pageSize);
        var fuelEntryResponses = fuelEntries
            .Select(fuelEntry => fuelEntry.ToResponse())
            .ToList();

        return Result<PagedResponse<FuelEntryResponse>>
            .Success(new PagedResponse<FuelEntryResponse>(
                fuelEntryResponses, page, pageSize, totalCount, totalPages));
    }

    public async Task<Result> Remove(Guid fuelEntryId)
    {
        var userId = userContextService.GetUserId();
        var fuelEntryEntity = await unitOfWork.FuelEntryRepository.GetFuelEntry(fuelEntryId, userId);
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
        var userId = userContextService.GetUserId();
        var fuelEntryEntity = await unitOfWork.FuelEntryRepository.GetFuelEntry(fuelEntryId, userId);
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
        fuelEntryEntity.IsFullTank = payload.IsFullTank;
        
        await unitOfWork.CommitAsync();

        return Result<FuelEntryResponse>.Success(fuelEntryEntity.ToResponse());
    }
}