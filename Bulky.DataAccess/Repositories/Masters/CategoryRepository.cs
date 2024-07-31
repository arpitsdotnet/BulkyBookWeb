using Bulky.DataAccess.Abstracts.Masters;
using Bulky.DataAccess.Base;
using Bulky.Models.Masters;

namespace Bulky.DataAccess.Repositories.Masters;
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
