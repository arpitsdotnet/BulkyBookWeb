using BulkyBook.DataAccess.Abstracts;
using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.DataAccess.Repositories.Masters;

namespace BulkyBook.DataAccess.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        Category = new CategoryRepository(_dbContext);
        Product = new ProductRepository(_dbContext);
        Company = new CompanyRepository(_dbContext);
        ShoppingCart = new ShoppingCartRepository(_dbContext);
        ApplicationUser = new ApplicationUserRepository(_dbContext);
    }


    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}
