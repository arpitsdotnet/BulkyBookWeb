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

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var existingOrder = _dbContext.OrderHeaders.FirstOrDefault(o => o.Id == id);

        if (existingOrder == null)
            throw new Exception($"Unable to find order for identifier {id}");

        if (!string.IsNullOrEmpty(paymentStatus))
            existingOrder.PaymentStatus = paymentStatus;

        existingOrder.OrderStatus = orderStatus;

        _dbContext.OrderHeaders.Update(existingOrder);
    }

    public void UpdateStipePaymentID(int id, string sessionId, string paymentIntentId)
    {
        var existingOrder = _dbContext.OrderHeaders.FirstOrDefault(o => o.Id == id);

        if (existingOrder == null)
            throw new Exception($"Unable to find order for identifier {id}");

        if (!string.IsNullOrEmpty(sessionId))
            existingOrder.SessionId = sessionId;

        if (!string.IsNullOrEmpty(paymentIntentId))
            existingOrder.PaymentIntentId = paymentIntentId;

        existingOrder.PaymentDate = DateTime.Now;

        _dbContext.OrderHeaders.Update(existingOrder);
    }
}
