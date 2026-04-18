namespace FuelTracker.API.Models;

public record AverageConsumptionResponse(
    decimal? AverageConsumption,
    int? TotalDistanceKm,
    decimal? TotalLiters,
    DateOnly? CalculatedFrom,
    DateOnly? CalculatedTo,
    int FullFillUpCount,
    int PartialFillUpCount,
    string? Message = null);
