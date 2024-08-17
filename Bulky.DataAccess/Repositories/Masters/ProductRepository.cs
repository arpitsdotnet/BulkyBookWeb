using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(Product product)
    {
        var existingProduct = _dbContext.Products.FirstOrDefault(x => x.Id == product.Id);
        if (existingProduct != null)
        {
            existingProduct.Title = product.Title;
            existingProduct.Description = product.Description;
            existingProduct.ISBN = product.ISBN;
            existingProduct.Price = product.Price;
            existingProduct.Price50 = product.Price50;
            existingProduct.Price100 = product.Price100;
            existingProduct.ListPrice = product.ListPrice;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Author = product.Author;
            existingProduct.ProductImages = product.ProductImages;
            //if (product.ImageUrl != null)
            //{
            //    existingProduct.ImageUrl = product.ImageUrl;
            //}
        }
    }
}
