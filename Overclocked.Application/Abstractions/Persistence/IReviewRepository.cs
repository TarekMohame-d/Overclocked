using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IReviewRepository : IGenericRepository<Domain.ReviewAggregate.Review, ReviewId>
{
    Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(
        ProductId productId,
        CancellationToken cancellationToken = default);
}
