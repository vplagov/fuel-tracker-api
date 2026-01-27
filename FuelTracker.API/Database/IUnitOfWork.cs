using FuelTracker.API.Repositories;

namespace FuelTracker.API.Database;

public interface IUnitOfWork
{
    CarRepository CarRepository { get; }
    FuelEntryRepository FuelEntryRepository { get; }
    UserRepository UserRepository { get; }

    Task CommitAsync();
}