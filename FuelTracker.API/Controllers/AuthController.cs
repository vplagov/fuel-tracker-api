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
}