using System.Linq.Expressions;
using Overclocked.Domain.ReviewAggregate;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IReviewRepository : IRepository
{
    Task<Review?> GetAsync(Expression<Func<Review, bool>> predicate, CancellationToken ct = default);

    Task<bool> ExistsAsync(Expression<Func<Review, bool>> predicate, CancellationToken ct = default);

    void Add(Review review);

    void Remove(Review review);
}
