using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface IOrderHeaderRepository : IRepository<OrderHeader>
{
    void Update(OrderHeader orderHeader);
}
