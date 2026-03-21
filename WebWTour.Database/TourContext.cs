using Microsoft.EntityFrameworkCore;
using WebWTour.Models;

namespace WebWTour.Database;

public class TourContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseSqlite("Data Source=tour.db");
    }
}