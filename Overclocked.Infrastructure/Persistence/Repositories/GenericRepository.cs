using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class GenericRepository<T, TId>(ApplicationDbContext context) : IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : IEntityKey
{
    protected readonly ApplicationDbContext _dbContext = context;
    protected readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

    public async Task<int> DeleteWhereAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(cancellationToken);
    }
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return _dbSet.AnyAsync(predicate, cancellationToken);
    }
}
