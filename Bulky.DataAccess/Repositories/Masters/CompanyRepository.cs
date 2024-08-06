using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class CompanyRepository : Repository<Company>, ICompanyRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(Company company)
    {
        _dbContext.Companies.Update(company);
    }
}
