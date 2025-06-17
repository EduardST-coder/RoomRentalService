using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RoomRental.DAL;
using RoomRental.DAL.Models;
using RoomRental.BLL.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔧 Підключення AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 Додавання Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ⚠️ Спрощені правила пароля (опціонально для тесту)
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); // Razor Pages підтримка

// 🔧 DI сервіс
builder.Services.AddScoped<RoomService>();

// 🔧 MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🧠 Seeder для ролі Admin і користувача
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Створити роль Admin
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Дані адміністратора
    const string adminEmail = "stugaed@gmail.com";
    const string adminPass = "edyard2020";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminPass);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"❌ Адмін не створений: {error.Description}");
            }
        }
    }
}

// 🔧 Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // обов’язково перед Authorization
app.UseAuthorization();

// 📌 маршрути
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Room}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
