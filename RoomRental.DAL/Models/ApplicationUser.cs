using Microsoft.AspNetCore.Identity;

namespace RoomRental.DAL.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty; // Посилання на фото
}
