using BulkyBook.Models.Identity;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface IApplicationUserRepository : IRepository<ApplicationUser>
{
    void Update(ApplicationUser user);
}
