using BulkyBook.Models.Masters;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Base;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryId = 1, CategoryName = "Action", CategoryDisplayOrder = 1 },
            new Category { CategoryId = 2, CategoryName = "SciFi", CategoryDisplayOrder = 2 },
            new Category { CategoryId = 3, CategoryName = "History", CategoryDisplayOrder = 3 }
            );
    }
}
