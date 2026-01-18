using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class OrderReadRepository(ApplicationDbContext dbContext) : IOrderReadRepository
{
    private readonly IQueryable<Order> _queryable = dbContext.Orders.AsNoTracking();

    public Task<Order?> GetByIdAsync(OrderId orderId, CancellationToken ct = default) =>
        _queryable.FirstOrDefaultAsync(x => x.Id == orderId, ct);

    public Task<int> CountAsync(UserId userId, int Year, CancellationToken ct = default)
    {
        IQueryable<Order> query = _queryable;
        query = query.Where(o => o.UserId == userId && o.CreatedAt.Year == Year);

        return query.CountAsync(ct);
    }

    public Task<List<Order>> GetPagedAsync(
        UserId userId,
        int pageNumber,
        int pageSize,
        int Year,
        SortDirection direction,
        CancellationToken ct = default
    )
    {
        IQueryable<Order> query = _queryable;

        query = query.Where(o => o.UserId == userId && o.CreatedAt.Year == Year);

        query = ApplySorting(query, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(ct);
    }

    private static IQueryable<Order> ApplySorting(IQueryable<Order> query, SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt);
    }
}
