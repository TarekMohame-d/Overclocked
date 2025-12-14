using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ReviewRepository(ApplicationDbContext context)
    : GenericRepository<Review, ReviewId>(context), IReviewRepository
{
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
}
