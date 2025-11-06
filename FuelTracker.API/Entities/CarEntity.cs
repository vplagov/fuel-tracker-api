namespace FuelTracker.API.Models;

public class CarEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<FuelEntry>  FuelEntries { get; set; } = new List<FuelEntry>();
}