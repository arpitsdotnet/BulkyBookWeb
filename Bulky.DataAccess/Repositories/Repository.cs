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
        _dbContext.Products.Include(u => u.Category).Include(u => u.CategoryId);
    }
    public void Add(T item)
    {
        dbSet.Add(item);
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;

        if (filter != null)
            query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var property in includeProperties
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(property);
            }
        }

        return query.ToList();
    }

    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
    {
        IQueryable<T> query;
        if (tracked)
        {
            query = dbSet;
        }
        else
        {
            query = dbSet.AsNoTracking();
        }

        query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            foreach (var property in includeProperties
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(property);
            }
        }

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
