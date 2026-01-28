using System.ComponentModel.DataAnnotations;

namespace FuelTracker.API.Configuration;

public record JwtSettings(
    [Required]
    [MinLength(32)]
    string Secret, 
    
    [Required]
    string Issuer, 
    
    [Required]
    string Audience, 
    
    [Required]
    [Range(1, 60)]
    int ExpirationMinutes);