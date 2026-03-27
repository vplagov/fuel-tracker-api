namespace FuelTracker.API.Models;

public record FuelEntryRequest(
    DateOnly Date,
    int Odometer,
    decimal Liters,
    decimal PricePerLiter);