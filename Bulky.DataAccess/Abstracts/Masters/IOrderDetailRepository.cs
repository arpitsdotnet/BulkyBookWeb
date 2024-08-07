using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Abstracts.Masters;
public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
}
