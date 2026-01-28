namespace FuelTracker.API.Models;

public record LoginResponse(
    Guid Id, 
    string Username,
    string Token);