using System.Linq.Expressions;
using BulkyBook.DataAccess.Abstracts;
using BulkyBook.DataAccess.Base;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    internal DbSet<T> dbSet { get; set; }

    private readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        dbSet = _dbContext.Set<T>();
    }
    public void Add(T item)
    {
        dbSet.Add(item);
    }

    public IEnumerable<T> GetAll()
    {
        IQueryable<T> query = dbSet;
        return query.ToList();
    }

    public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);

        return query.FirstOrDefault();
    }

    public void Remove(T item)
    {
        dbSet.Remove(item);
    }

    public void RemoveRange(IEnumerable<T> items)
    {
        dbSet.RemoveRange(items);
    }
}
