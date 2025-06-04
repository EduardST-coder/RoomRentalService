using Microsoft.EntityFrameworkCore;
using RoomRental.DAL;
using RoomRental.BLL.Services; // ✅ Підключаємо простір імен для RoomService

var builder = WebApplication.CreateBuilder(args);

// ✅ Додаємо AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Реєструємо RoomService (інжекція залежностей)
builder.Services.AddScoped<RoomService>();

// ✅ (Опціонально) AutoMapper, якщо буде потрібно
// builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔐 Обробка помилок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // 🔧 підключення wwwroot

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
