namespace FuelTracker.API.Models;

public record RegisterRequest(string Username, string Password, string? Email);