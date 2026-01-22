using FuelTracker.API.Models;
using FuelTracker.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace FuelTracker.API.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController(AuthService authService) : BaseController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterUser(request);
        return result.IsFailure ? 
            HandleFailure(result) : 
            StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await authService.Login(loginRequest);
        return result.IsFailure ? HandleFailure(result) : Ok(result.Value);
    }
}