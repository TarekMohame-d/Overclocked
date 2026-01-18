using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    private readonly DbSet<Order> _dbSet = dbContext.Orders;

    public Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken ct = default) => _dbSet.FindAsync([orderId], ct).AsTask();

    public Task<Order?> GetByUserIdAsync(UserId userId, CancellationToken ct = default) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public Task<List<Order>> GetPendingOrdersOlderThanAsync(
        DateTimeOffset cutoffTime,
        int batchSize,
        CancellationToken ct = default
    )
    {
        return _dbSet
            .AsTracking()
            .Where(o => o.Status == OrderStatus.PendingPayment && o.CreatedAt < cutoffTime)
            .OrderBy(o => o.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public Task<List<Order>> GetPlacedOrdersOlderThanAsync(
        DateTimeOffset cutoffTime,
        int batchSize,
        CancellationToken ct = default
    )
    {
        return _dbSet
            .AsTracking()
            .Where(o => o.Status == OrderStatus.Placed && o.CreatedAt < cutoffTime)
            .OrderBy(o => o.CreatedAt)
            .Take(batchSize)
            .ToListAsync(ct);
    }

    public void Add(Order order) => _dbSet.Add(order);
}
