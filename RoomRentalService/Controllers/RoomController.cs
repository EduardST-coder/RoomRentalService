using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using RoomRental.BLL.Services;
using RoomRental.Models;
using RoomRental.DAL.Models;
using System.Security.Claims;

namespace RoomRentalService.Controllers;

public class RoomController : Controller
{
    private readonly RoomService _roomService;
    private readonly UserManager<ApplicationUser> _userManager;

    public RoomController(RoomService roomService, UserManager<ApplicationUser> userManager)
    {
        _roomService = roomService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var rooms = await _roomService.GetAllAsync();
        return View(rooms);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Room room, List<IFormFile> photos)
    {
        if (!ModelState.IsValid)
            return View(room);

        room.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        room.CreatedAt = DateTime.Now;

        await _roomService.AddAsync(room); // зберігає Room і створює Id

        if (photos != null && photos.Any())
        {
            var folderPath = Path.Combine("wwwroot", "uploads", room.Id.ToString());
            Directory.CreateDirectory(folderPath);

            for (int i = 0; i < photos.Count; i++)
            {
                var file = photos[i];

                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    room.Images.Add(new RoomImage
                    {
                        RoomId = room.Id,
                        ImagePath = $"/uploads/{room.Id}/{fileName}",
                        IsMain = i == 0 // перше фото — головне
                    });
                }
            }

            await _roomService.UpdateAsync(room);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null) return NotFound();
        return View(room);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Room room)
    {
        if (!ModelState.IsValid)
            return View(room);

        await _roomService.UpdateAsync(room);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null) return NotFound();
        return View(room);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _roomService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var room = await _roomService.GetByIdAsync(id);
        if (room == null) return NotFound();

        var owner = await _userManager.FindByIdAsync(room.OwnerId);
        ViewBag.Owner = owner;

        return View(room);
    }
}
