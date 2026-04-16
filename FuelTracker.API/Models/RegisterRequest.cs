using System.ComponentModel.DataAnnotations;

namespace FuelTracker.API.Models;

public record RegisterRequest(
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]
    string Username,
    
    [Required]
    string Password,
    
    [EmailAddress]
    string? Email);