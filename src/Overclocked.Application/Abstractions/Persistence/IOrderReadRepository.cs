using Overclocked.Application.Common.Enums;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IOrderReadRepository : IRepository
{
    Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken ct = default);

    Task<int> CountAsync(UserId userId, int Year, CancellationToken ct = default);

    Task<List<Order>> GetPagedAsync(
        UserId userId,
        int pageNumber,
        int pageSize,
        int Year,
        SortDirection direction,
        CancellationToken ct = default
    );
}
