using Application.Common.Enums;
using Domain.Entities;

namespace Application.Abstraction.Repositories;

public interface IReviewRepository : IGenericRepository<Review>
{
    Task<IDictionary<int, int>> GetProductRatingsBreakdownAsync(
        Guid productId,
        CancellationToken cancellationToken = default);

    IQueryable<Review> GetReviewsQuery(Guid prodGuid, ReviewSortField sortBy, SortDirection direction);
}
