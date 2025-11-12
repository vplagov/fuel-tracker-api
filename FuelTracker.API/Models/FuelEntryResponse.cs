namespace FuelTracker.API.Models;

public record FuelEntryResponse(
    Guid Id,
    DateTime Date,
    int Odometer,
    decimal Liters,
    decimal PricePerLiter,
    decimal TotalCost);