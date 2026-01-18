using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IOrderRepository : IRepository
{
    Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken ct = default);
    Task<Order?> GetByUserIdAsync(UserId userId, CancellationToken ct = default);
    Task<List<Order>> GetPendingOrdersOlderThanAsync(DateTimeOffset cutoffTime, int batchSize, CancellationToken ct = default);
    Task<List<Order>> GetPlacedOrdersOlderThanAsync(DateTimeOffset cutoffTime, int batchSize, CancellationToken ct = default);
    void Add(Order order);
}
