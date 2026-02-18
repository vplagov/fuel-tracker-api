using System.ComponentModel.DataAnnotations;

namespace FuelTracker.API.Configuration;

public record JwtSettings
{
    [Required]
    [MinLength(32)]
    public required string Secret { get; init; }
    
    [Required]
    public required string Issuer { get; init; }
    
    [Required]
    public required string Audience { get; init; }
    
    [Required]
    [Range(1, 60)]
    public required int ExpirationMinutes { get; init; }
}