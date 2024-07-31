using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
    void SaveChanges();
}
