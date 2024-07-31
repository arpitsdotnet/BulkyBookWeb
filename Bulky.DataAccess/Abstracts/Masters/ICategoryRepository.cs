using Bulky.Models.Masters;

namespace Bulky.DataAccess.Abstracts.Masters;
public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
    void SaveChanges();
}
