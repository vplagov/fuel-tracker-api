namespace FuelTracker.API.Models;

public record FuelEntryRequest(
    DateTime Date,
    int Odometer,
    decimal Liters,
    decimal PricePerLiter);