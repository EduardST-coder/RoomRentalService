using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.DAL;
using RoomRental.Models;
using System.Security.Claims;

namespace RoomRentalService.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Cabinet()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var myRooms = await _context.Rooms
            .Where(r => r.OwnerId == userId)
            .ToListAsync();

        // TODO: додати заявки, коли реалізуємо модель Request

        ViewBag.MyRooms = myRooms;
        return View();
    }
}
