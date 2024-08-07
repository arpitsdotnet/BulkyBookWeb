using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(OrderDetail orderDetail)
    {
        _dbContext.OrderDetails.Update(orderDetail);
    }
}
