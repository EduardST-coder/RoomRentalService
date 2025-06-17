namespace RoomRental.Models;

public class Room
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerDay { get; set; }
    public bool IsAvailable { get; set; } = true;

    public string OwnerId { get; set; } = string.Empty;

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();


    // 🔄 Нові поля для головної сторінки
    public string Location { get; set; } = string.Empty;
    public double SquareMeters { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
