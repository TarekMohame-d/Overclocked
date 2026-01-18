using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ReviewReadRepository(ApplicationDbContext dbContext) : IReviewReadRepository
{
    private readonly IQueryable<Review> _queryable = dbContext.Reviews.AsNoTracking();

    public async Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(ProductId productId, CancellationToken ct = default)
    {
        var grouped = await _queryable
            .Where(r => r.ProductId == productId)
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var result = new Dictionary<int, int>
        {
            { 1, 0 },
            { 2, 0 },
            { 3, 0 },
            { 4, 0 },
            { 5, 0 },
        };

        foreach (var g in grouped)
        {
            if (g.Rating is >= 1 and <= 5)
                result[g.Rating] = g.Count;
        }

        return result;
    }

    public Task<int> CountAsync(ProductId productId, CancellationToken ct = default) =>
        _queryable.CountAsync(x => x.ProductId == productId, ct);

    public Task<List<Review>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ReviewSortField sortBy,
        SortDirection direction,
        CancellationToken ct = default
    )
    {
        IQueryable<Review> query = _queryable;

        query = query.Include(r => r.User).Include(r => r.ReviewReply);

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(ct);
    }

    private static IQueryable<Review> ApplySorting(IQueryable<Review> query, ReviewSortField sortBy, SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            ReviewSortField.Rating => isDescending ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating),

            ReviewSortField.CreatedAt => isDescending
                ? query.OrderByDescending(p => p.UpdatedAt)
                : query.OrderBy(p => p.UpdatedAt),

            ReviewSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }
}
