using Microsoft.EntityFrameworkCore;
using RoomRental.DAL;
using RoomRental.Models;

namespace RoomRental.BLL.Services;

public class RoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.Images)
            .ToListAsync();
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(Room room)
    {
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room != null)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
    }

    // ✅ Новий метод для отримання ID вибраних кімнат користувача
    public async Task<List<int>> GetFavoriteRoomIdsAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Select(f => f.RoomId)
            .ToListAsync();
    }
}
