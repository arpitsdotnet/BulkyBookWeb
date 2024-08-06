using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface ICompanyRepository : IRepository<Company>
{
    void Update(Company company);
}
