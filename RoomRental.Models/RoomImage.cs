namespace RoomRental.Models;

public class RoomImage
{
    public int Id { get; set; }

    public string ImagePath { get; set; } = string.Empty;

    public bool IsMain { get; set; } = false; // 🔑 нове поле

    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;
}
