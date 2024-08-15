using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Identity;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.DBInitializers;
public sealed class DBInitializer : IDbInitializer
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public DBInitializer(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public void Initialize()
    {
        //migrations if they are not applied
        try
        {
            if (_context.Database.GetPendingMigrations().Count() > 0)
            {
                _context.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
        }

        if (!_roleManager.RoleExistsAsync(SD.Role.Customer).GetAwaiter().GetResult())
        {
            //create roles if they are not created
            _roleManager.CreateAsync(new IdentityRole(SD.Role.Customer)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role.Company)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Role.Employee)).GetAwaiter().GetResult();

            //if roles are not created, then we will create admin user as well.
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@finazzy.com",
                Email = "admin@finazzy.com",
                Name = "Arpit Shrivastava",
                PhoneNumber = "1234567890",
                StreetAddress = "101, Shreeji Valley",
                City = "Indore",
                State = "MP",
                PostalCode = "452016"
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser? user = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@finazzy.com");

            _userManager.AddToRoleAsync(user, SD.Role.Admin).GetAwaiter().GetResult();
        }

        return;
    }
}
