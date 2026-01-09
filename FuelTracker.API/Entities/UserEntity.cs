namespace FuelTracker.API.Entities;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime? CreatedAt { get; set; }
    
    public ICollection<CarEntity> CarEntities { get; set; } = new List<CarEntity>();
}