using System.Linq.Expressions;

namespace BulkyBook.DataAccess.Abstracts;
public interface IRepository<T> where T:class
{
    IEnumerable<T> GetAll(string? includeProperties = null);
    T Get(Expression<Func<T,bool>> filter, string? includeProperties = null);

    void Add(T item);
    //void Update(T item); SaveChanges 
    void Remove(T item);
    void RemoveRange(IEnumerable<T> items);
}
