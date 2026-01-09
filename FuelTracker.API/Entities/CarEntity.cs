namespace FuelTracker.API.Entities;

public class CarEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    public ICollection<FuelEntry> FuelEntries { get; set; } = new List<FuelEntry>();

    public UserEntity UserEntity { get; set; } = null!;
}