using FuelTracker.API.Entities;
using FuelTracker.API.Models;

namespace FuelTracker.API.Extensions;

public static class MappingExtensions
{
    public static CarResponse ToResponse(this CarEntity entity)
    {
        return new CarResponse(entity.Id, entity.Name);
    }

    public static UserResponse ToResponse(this UserEntity entity)
    {
        return new UserResponse(entity.Id, entity.Username);
    }

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