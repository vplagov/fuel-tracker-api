using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace FuelTracker.API.Repositories;

public class UserRepository(FuelTrackerContext context)
{
    public Task<bool> ExistsAsync(string username)
    {
        return context.Users.AnyAsync(u => u.Username == username);
    }

    public void Add(UserEntity userEntity)
    {
        context.Users.Add(userEntity);
    }

    public Task<UserEntity?> GetByUsernameAsync(string username)
    {
        return context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == username);
    }
}