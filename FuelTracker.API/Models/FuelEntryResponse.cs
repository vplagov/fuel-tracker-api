namespace FuelTracker.API.Models;

public record FuelEntryResponse(
    Guid Id,
    DateOnly Date,
    int Odometer,
    decimal Liters,
    decimal PricePerLiter,
    decimal TotalCost,
    bool IsFullTank);