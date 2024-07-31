using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }

    public void Update(Category category)
    {
        _dbContext.Categories.Update(category);
    }
}
