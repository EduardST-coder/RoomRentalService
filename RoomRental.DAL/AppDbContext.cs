using Microsoft.EntityFrameworkCore;
using RoomRental.Models;


namespace RoomRental.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Rental> Rentals => Set<Rental>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // зв'язки
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
