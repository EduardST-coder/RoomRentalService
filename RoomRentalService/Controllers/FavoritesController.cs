using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.DAL;
using RoomRental.Models;
using System.Security.Claims;

namespace RoomRentalService.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly AppDbContext _context;

    public FavoritesController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ Додавання до вибраного
    [HttpPost]
    public async Task<IActionResult> Add(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var exists = await _context.Favorites
            .AnyAsync(f => f.RoomId == id && f.UserId == userId);

        if (!exists)
        {
            var favorite = new Favorite
            {
                RoomId = id,
                UserId = userId
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Room");
    }

    // ✅ Видалення з вибраного
    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.RoomId == id && f.UserId == userId);

        if (favorite != null)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Room");
    }

    // ✅ Перегляд списку вибраного
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Room)
            .ThenInclude(r => r.Images)
            .ToListAsync();

        return View(favorites);
    }
}
