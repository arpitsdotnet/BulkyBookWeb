using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Identity;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(ApplicationUser user)
    {
        _dbContext.ApplicationUsers.Update(user);
    }
}
