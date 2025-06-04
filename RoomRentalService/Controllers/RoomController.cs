using Microsoft.AspNetCore.Mvc;
using RoomRental.BLL.Services;
using RoomRental.Models;

namespace RoomRentalService.Controllers;

public class RoomController : Controller
{
    private readonly RoomService _roomService;

    public RoomController(RoomService roomService)
    {
        _roomService = roomService;
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
    public async Task<IActionResult> Create(Room room)
    {
        if (!ModelState.IsValid)
            return View(room);

        await _roomService.AddAsync(room);
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
        await _roomService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
