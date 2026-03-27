namespace FuelTracker.API.Entities;

public class FuelEntry
{
    public Guid Id { get; set; }
    public Guid CarId { get; set; }
    public DateOnly Date { get; set; }
    public int Odometer { get; set; }
    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalCost { get; set; }

    public CarEntity CarEntity { get; set; } = null!;
}