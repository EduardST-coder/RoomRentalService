using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomRental.DAL;
using RoomRental.DAL.Models;
using RoomRental.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace RoomRentalService.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(AppDbContext context, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class EditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public IFormFile? Avatar { get; set; }
    }

    [AllowAnonymous]
    public IActionResult Register() => View();

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Room");
        }

        foreach (var error in result.Errors)
        {
            Console.WriteLine($"❌ Реєстрація помилка: {error.Description}");
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Login() => View();

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded)
            return RedirectToAction("Index", "Room");

        ModelState.AddModelError("", "Невірний логін або пароль");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Room");
    }

    public async Task<IActionResult> Cabinet()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var myRooms = await _context.Rooms
            .Where(r => r.OwnerId == userId)
            .ToListAsync();

        var unreadCount = await _context.Messages
            .Where(m => m.RecipientId == userId && !m.IsRead)
            .CountAsync();

        ViewBag.MyRooms = myRooms;
        ViewBag.UnreadCount = unreadCount;

        return View();
    }

    public async Task<IActionResult> Notifications()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(notifications);
    }

    public async Task<IActionResult> Favorites()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var favorites = await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Room)
            .ToListAsync();

        return View(favorites);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new EditProfileViewModel
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;

        if (model.Avatar != null && model.Avatar.Length > 0)
        {
            var uploadsFolder = Path.Combine("wwwroot", "uploads", "avatars");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{user.Id}{Path.GetExtension(model.Avatar.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.Avatar.CopyToAsync(stream);
            }

            user.AvatarUrl = $"/uploads/avatars/{fileName}";
        }

        await _userManager.UpdateAsync(user);
        return RedirectToAction("Cabinet");
    }
}
