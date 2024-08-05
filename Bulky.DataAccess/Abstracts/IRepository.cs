using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Abstracts;
public interface IRepository<T> where T:class
{
    IEnumerable<T> GetAll(string? includeProperties = null);
    T GetFirstOrDefault(Expression<Func<T,bool>> filter, string? includeProperties = null);

    void Add(T item);
    //void Update(T item); SaveChanges 
    void Remove(T item);
    void RemoveRange(IEnumerable<T> items);
}
