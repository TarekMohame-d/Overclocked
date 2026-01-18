using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ReviewAggregate;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ReviewRepository(ApplicationDbContext dbContext) : IReviewRepository
{
    private readonly DbSet<Review> _dbSet = dbContext.Reviews;

    public Task<Review?> GetAsync(Expression<Func<Review, bool>> predicate, CancellationToken ct = default) =>
        _dbSet.AsTracking().FirstOrDefaultAsync(predicate, ct);

    public Task<bool> ExistsAsync(Expression<Func<Review, bool>> predicate, CancellationToken ct = default) =>
        _dbSet.AnyAsync(predicate, ct);

    public Task<Review?> GetForUpdateAsync(Expression<Func<Review, bool>> predicate, CancellationToken ct = default) =>
        _dbSet.AsTracking().Include(r => r.ReviewReply).FirstOrDefaultAsync(predicate, ct);

    public void Add(Review review) => _dbSet.Add(review);

    public void Remove(Review review) => _dbSet.Remove(review);
}
