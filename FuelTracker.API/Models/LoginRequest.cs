using System.ComponentModel.DataAnnotations;

namespace FuelTracker.API.Models;

public record LoginRequest(
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z0-9_]+$")]
    string Username,
    
    [Required]
    string Password);