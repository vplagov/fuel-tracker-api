using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Models;
using FuelTracker.API.Repositories;
using FuelTracker.API.Security;
using FuelTracker.API.Shared;

namespace FuelTracker.API.Services;

public class AuthService(UserRepository userRepository, FuelTrackerContext context)
{
    public async Task<Result<UserResponse>> RegisterUser(RegisterRequest registerRequest)
    {
        var isExist = await userRepository.ExistsAsync(registerRequest.Username);
        if (isExist)
        {
            return Result<UserResponse>.Failure(ErrorType.Conflict,
                $"User with username {registerRequest.Username} already exists.");
        }

        var userEntity = new UserEntity()
        {
            Id = Guid.NewGuid(),
            Username = registerRequest.Username,
            PasswordHash = PasswordHasher.HashPassword(registerRequest.Password),
            Email = registerRequest.Email,
            CreatedAt = DateTime.UtcNow
        };
        
        userRepository.Add(userEntity);
        await context.SaveChangesAsync();

        var userResponse = new UserResponse(userEntity.Id, userEntity.Username);
        return Result<UserResponse>.Success(userResponse);
    }
}