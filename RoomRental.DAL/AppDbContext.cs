using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomRental.Models;
using RoomRental.DAL.Models;

namespace RoomRental.DAL;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<RoomImage> RoomImages { get; set; }
    public DbSet<Message> Messages { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Room)
            .WithMany(rm => rm.Rentals)
            .HasForeignKey(r => r.RoomId);

        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.Rentals)
            .HasForeignKey(r => r.CustomerId);
    }
}
