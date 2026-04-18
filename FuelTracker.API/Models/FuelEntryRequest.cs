using System.ComponentModel.DataAnnotations;

namespace FuelTracker.API.Models;

public record FuelEntryRequest(
    [Required] DateOnly Date,
    [Range(0, 1_000_000)] int Odometer,
    [Range(0.01, 1000)] decimal Liters,
    [Range(0.01, 100)] decimal PricePerLiter,
    bool IsFullTank = true);