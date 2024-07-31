using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Abstracts;
public interface IRepository<T> where T:class
{
    IEnumerable<T> GetAll();
    T GetFirstOfDefault(Expression<Func<T,bool>> filter);

    void Add(T item);
    //void Update(T item); SaveChanges 
    void Remove(T item);
    void RemoveRange(IEnumerable<T> items);
}
