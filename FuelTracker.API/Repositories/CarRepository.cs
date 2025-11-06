using FuelTracker.API.Database;
using FuelTracker.API.Models;

namespace FuelTracker.API.Repository;

public class CarRepository
{
    private readonly FuelTrackerContext  _context;
    
    public CarRepository(FuelTrackerContext context)
    {
        _context = context;
    }

    public void Add(CarEntity carEntity)
    {
        _context.Cars.Add(carEntity);
    }
}