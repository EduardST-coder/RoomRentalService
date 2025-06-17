using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.Models;
using RoomRental.DAL;
using RoomRental.DAL.Models;
using System.Security.Claims;

[Authorize]
public class MessageController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessageController(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Send(string recipientId, string content)
    {
        var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(recipientId))
            return BadRequest("Дані не заповнені.");

        var message = new Message
        {
            SenderId = senderId,
            RecipientId = recipientId,
            Content = content,
            SentAt = DateTime.Now,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return RedirectToAction("Chat", new { userId = recipientId });
    }

    [HttpGet]
    public async Task<IActionResult> Inbox()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var chatUserIds = await _context.Messages
            .Where(m => m.SenderId == userId || m.RecipientId == userId)
            .Select(m => m.SenderId == userId ? m.RecipientId : m.SenderId)
            .Distinct()
            .ToListAsync();

        var users = await _userManager.Users
            .Where(u => chatUserIds.Contains(u.Id))
            .ToListAsync();

        ViewBag.CurrentUserId = userId;

        // Передаємо всі повідомлення для позначки непрочитаних
        ViewBag.AllMessages = await _context.Messages
            .Where(m => m.RecipientId == userId && !m.IsRead)
            .ToListAsync();

        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Chat(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var messages = await _context.Messages
            .Where(m =>
                (m.SenderId == currentUserId && m.RecipientId == userId) ||
                (m.SenderId == userId && m.RecipientId == currentUserId))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        // ✅ Позначаємо як прочитані
        var unread = messages
            .Where(m => m.RecipientId == currentUserId && !m.IsRead)
            .ToList();

        if (unread.Any())
        {
            foreach (var msg in unread)
                msg.IsRead = true;

            await _context.SaveChangesAsync();
        }

        var user = await _userManager.FindByIdAsync(userId);
        ViewBag.OtherUser = user;

        return View(messages);
    }

    [HttpGet]
    public async Task<int> GetUnreadCount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var count = await _context.Messages
            .CountAsync(m => m.RecipientId == userId && !m.IsRead);
        return count;
    }
}
