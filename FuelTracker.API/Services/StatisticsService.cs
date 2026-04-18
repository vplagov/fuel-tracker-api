using FuelTracker.API.Database;
using FuelTracker.API.Models;
using FuelTracker.API.Shared;

namespace FuelTracker.API.Services;

public class StatisticsService(IUnitOfWork unitOfWork, UserContextService userContextService)
{
    public async Task<Result<AverageConsumptionResponse>> GetAverageConsumption(Guid carId)
    {
        var userId = userContextService.GetUserId();
        var car = await unitOfWork.CarRepository.GetCar(carId, userId);
        if (car == null)
        {
            return Result<AverageConsumptionResponse>.Failure(ErrorType.NotFound, $"Car with ID '{carId}' not found");
        }

        var entries = await unitOfWork.FuelEntryRepository.GetFuelEntries(carId);
        var sortedEntries = entries.OrderBy(e => e.Date).ThenBy(e => e.Odometer).ToList();

        var fullFillUps = sortedEntries.Where(e => e.IsFullTank).ToList();
        var fullCount = fullFillUps.Count;
        var partialCount = sortedEntries.Count - fullCount;

        if (fullCount < 2)
        {
            return Result<AverageConsumptionResponse>.Success(new AverageConsumptionResponse(
                null, null, null, null, null, fullCount, partialCount,
                "At least 2 full fill-up entries are required to calculate average consumption."));
        }

        var firstAnchor = fullFillUps.First();
        var lastAnchor = fullFillUps.Last();

        var firstAnchorIndex = sortedEntries.IndexOf(firstAnchor);
        var lastAnchorIndex = sortedEntries.LastIndexOf(lastAnchor);

        var includedEntries = sortedEntries.GetRange(firstAnchorIndex, lastAnchorIndex - firstAnchorIndex + 1);

        var totalDistance = lastAnchor.Odometer - firstAnchor.Odometer;
        
        // Sum liters for all entries between and including anchors, excluding the first anchor's liters
        var totalLiters = includedEntries.Sum(e => e.Liters) - firstAnchor.Liters;

        decimal? averageConsumption = null;
        if (totalDistance > 0)
        {
            averageConsumption = Math.Round((totalLiters / totalDistance) * 100, 2);
        }

        return Result<AverageConsumptionResponse>.Success(new AverageConsumptionResponse(
            averageConsumption,
            totalDistance,
            totalLiters,
            firstAnchor.Date,
            lastAnchor.Date,
            fullCount,
            partialCount));
    }
}
