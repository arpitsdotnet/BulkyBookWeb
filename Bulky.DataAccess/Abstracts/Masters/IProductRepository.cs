using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface IProductRepository : IRepository<Product>
{
    void Update(Product category);
}
