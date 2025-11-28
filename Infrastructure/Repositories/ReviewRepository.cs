using Application.Abstraction.Repositories;
using Application.Common.Enums;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ReviewRepository(ApplicationDbContext dbContext)
    : GenericRepository<Review>(dbContext), IReviewRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    public async Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        var grouped = await Query()
            .Where(r => r.ProductId == productId)
            .GroupBy(r => r.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = new Dictionary<int, int> { { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 } };

        foreach(var g in grouped)
        {
            if(g.Rating is >= 1 and <= 5)
                result[g.Rating] = g.Count;
        }

        return result;
    }

    public IQueryable<Review> GetReviewsQuery(Guid productId, ReviewSortField sortBy, SortDirection direction)
    {
        IQueryable<Review> query = _dbContext.Reviews.AsNoTracking();

        query
        .AsSplitQuery()
        .Where(r => r.ProductId == productId)
        .Include(r => r.User)
        .Include(r => r.ReviewReply);

        query = ApplySorting(query, sortBy, direction);

        return query;
    }

    private static IQueryable<Review> ApplySorting(IQueryable<Review> query, ReviewSortField sortBy, SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            ReviewSortField.CreatedAt => isDescending
                ? query.OrderByDescending(p => p.UpdatedAt) : query.OrderBy(p => p.UpdatedAt),

            ReviewSortField.Rating or _ => isDescending
                ? query.OrderByDescending(p => p.Rating) : query.OrderBy(p => p.Rating),
        };
    }
}
