using FuelTracker.API.Models;

namespace FuelTracker.API.Extensions;

public static class FuelEntryMappingExtensions
{
    public static FuelEntryResponse ToResponse(this FuelEntry entity)
    {
        return new FuelEntryResponse(
            entity.Id,
            entity.Date,
            entity.Odometer,
            entity.Liters,
            entity.PricePerLiter,
            entity.TotalCost);
    }
}