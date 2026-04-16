using System.ComponentModel.DataAnnotations;

namespace FuelTracker.API.Models;

public record CarRequest([Required] string Name);