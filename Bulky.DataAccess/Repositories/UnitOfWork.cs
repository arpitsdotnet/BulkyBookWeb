using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        Category = new CategoryRepository(_dbContext);
        Product = new ProductRepository(_dbContext);
        Company = new CompanyRepository(_dbContext);
    }


    public void SaveChanges()
    {
        _dbContext.SaveChanges();
    }
}
