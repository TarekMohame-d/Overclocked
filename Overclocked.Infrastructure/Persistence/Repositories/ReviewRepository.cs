using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ReviewRepository(ApplicationDbContext context)
    : GenericRepository<Review, ReviewId>(context), IReviewRepository
{
    public Task<Review?> GetById(
        Expression<Func<Review, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public Task<Review?> GetForUpdateAsync(
        Expression<Func<Review, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AsTracking()
            .Include(r => r.ReviewReply)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(
        ProductId productId,
        CancellationToken cancellationToken = default)
    {
        var grouped = await _dbSet
            .Where(r => r.ProductId == productId)
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };

        foreach(var g in grouped)
        {
            if(g.Rating is >= 1 and <= 5)
            {
                result[g.Rating] = g.Count;
            }
        }

        return result;
    }

    public Task<int> CountAsync(ProductId productId, CancellationToken cancellationToken = default)
    {
        IQueryable<Review> query = _dbSet.AsNoTracking();

        return query.CountAsync(x => x.ProductId == productId, cancellationToken);
    }

    public Task<List<Review>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ReviewSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Review> query = _dbSet.AsNoTracking();
        query = query.Include(p => p.User);

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(cancellationToken);
    }

    private static IQueryable<Review> ApplySorting(
        IQueryable<Review> query,
        ReviewSortField sortBy,
        SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            ReviewSortField.Rating => isDescending ? query.OrderByDescending(p => p.Rating)
            : query.OrderBy(p => p.Rating),

            ReviewSortField.CreatedAt => isDescending ? query.OrderByDescending(p => p.UpdatedAt)
            : query.OrderBy(p => p.UpdatedAt),

            ReviewSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }
}
