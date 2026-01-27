using FuelTracker.API.Repositories;

namespace FuelTracker.API.Database;

public class UnitOfWork(
    FuelTrackerContext context,
    CarRepository carRepository,
    FuelEntryRepository fuelEntryRepository,
    UserRepository userRepository) : IUnitOfWork
{
    public CarRepository CarRepository { get; } = carRepository;
    public FuelEntryRepository FuelEntryRepository { get; } = fuelEntryRepository;
    public UserRepository UserRepository { get; } = userRepository;

    public Task CommitAsync() => context.SaveChangesAsync();
}