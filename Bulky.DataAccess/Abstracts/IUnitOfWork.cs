using BulkyBook.DataAccess.Abstracts.Masters;

namespace BulkyBook.DataAccess.Abstracts;
public interface IUnitOfWork
{
    ICategoryRepository Category { get; }
    IProductRepository Product { get; }
    ICompanyRepository Company { get; }
    void SaveChanges();
}
