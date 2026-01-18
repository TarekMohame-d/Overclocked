using Overclocked.Application.Common.Enums;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IReviewReadRepository : IRepository
{
    Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(ProductId productId, CancellationToken ct = default);

    Task<int> CountAsync(ProductId productId, CancellationToken ct = default);

    Task<List<Review>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ReviewSortField sortBy,
        SortDirection direction,
        CancellationToken ct = default
    );
}
