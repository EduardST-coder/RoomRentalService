namespace RoomRental.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerDay { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}
