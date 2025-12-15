using System.Linq.Expressions;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IReviewRepository : IGenericRepository<ReviewEntity, ReviewId>
{
    Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(
        ProductId productId,
        CancellationToken cancellationToken = default);

    Task<ReviewEntity?> GetForUpdateAsync(
        Expression<Func<ReviewEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<ReviewEntity?> GetById(
        Expression<Func<ReviewEntity, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(ProductId productId, CancellationToken cancellationToken = default);
    Task<List<ReviewEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ReviewSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default);
}
