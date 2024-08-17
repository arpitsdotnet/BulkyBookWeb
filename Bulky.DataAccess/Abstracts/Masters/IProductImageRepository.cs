using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface IProductImageRepository : IRepository<ProductImage>
{
    void Update(ProductImage productImage);
}
