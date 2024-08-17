using BulkyBook.DataAccess.Abstracts.Masters;

namespace BulkyBook.DataAccess.Abstracts;
public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    IProductImageRepository ProductImage { get; }
    ICompanyRepository Company { get; }

    IApplicationUserRepository ApplicationUser { get; }

    IShoppingCartRepository ShoppingCart { get; }


    IOrderHeaderRepository OrderHeader { get; }
    IOrderDetailRepository OrderDetail { get; }

    void SaveChanges();
}
