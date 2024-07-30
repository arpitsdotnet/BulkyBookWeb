using Bulky.WebUI.Models.Masters;
using Microsoft.EntityFrameworkCore;

namespace Bulky.WebUI.Data.Base;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
}
