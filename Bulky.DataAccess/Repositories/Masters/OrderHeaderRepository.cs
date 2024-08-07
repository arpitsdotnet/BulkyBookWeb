using BulkyBook.DataAccess.Abstracts.Masters;
using BulkyBook.DataAccess.Base;
using BulkyBook.Models.Masters;

namespace BulkyBook.DataAccess.Repositories.Masters;
public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
{
    private readonly ApplicationDbContext _dbContext;

    public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public void Update(OrderHeader orderHeader)
    {
        _dbContext.OrderHeaders.Update(orderHeader);
    }
}
