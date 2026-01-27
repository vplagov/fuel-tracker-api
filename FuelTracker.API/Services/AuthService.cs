using FuelTracker.API.Database;
using FuelTracker.API.Entities;
using FuelTracker.API.Models;
using FuelTracker.API.Security;
using FuelTracker.API.Shared;

namespace FuelTracker.API.Services;

public class AuthService(IUnitOfWork unitOfWork)
{
    public async Task<Result<UserResponse>> RegisterUser(RegisterRequest registerRequest)
    {
        var isExist = await unitOfWork.UserRepository.ExistsAsync(registerRequest.Username);
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
        
        unitOfWork.UserRepository.Add(userEntity);
        await unitOfWork.CommitAsync();

        var userResponse = new UserResponse(userEntity.Id, userEntity.Username);
        return Result<UserResponse>.Success(userResponse);
    }

    public async Task<Result<LoginResponse>> Login(LoginRequest loginRequest)
    {
        var userEntity = await unitOfWork.UserRepository.GetByUsernameAsync(loginRequest.Username);
        if (userEntity == null || !PasswordHasher.VerifyHashedPassword(loginRequest.Password, userEntity.PasswordHash))
        {
            return Result<LoginResponse>.Failure(ErrorType.Unauthorized, "Invalid username or password");
        }

        var userResponse = new LoginResponse(userEntity.Id, userEntity.Username);
        return Result<LoginResponse>.Success(userResponse);
    }
}