using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductImageRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(ProductImage productImage)
    {
        _dbContext.ProductImages.Update(productImage);
    }
}
