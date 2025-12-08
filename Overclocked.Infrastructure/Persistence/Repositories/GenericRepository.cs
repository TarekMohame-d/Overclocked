using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class GenericRepository<T, TId>(ApplicationDbContext context) : IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : IEntityKey
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public async Task<IEnumerable<T>> GetAllAsync(
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();
        return await query.ToListAsync(cancellationToken);
    }

    public IQueryable<T> Query(bool asNoTracking = true) =>
        asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();

    public async Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default) =>
        await _dbSet.FindAsync([id], cancellationToken);

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();

        if(include is not null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();
        return await query.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<T?> SingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();

        if(include is not null)
        {
            query = include(query);
        }

        return await query.SingleOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> WhereAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();
        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> WhereAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = asNoTracking ? _dbSet.AsNoTracking() : _dbSet.AsTracking();

        if(include is not null)
        {
            query = include(query);
        }

        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        EntityEntry<T> entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default) =>
        await _dbSet.AddRangeAsync(entities, cancellationToken);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<T> entities) => _dbSet.RemoveRange(entities);

    public async Task<int> DeleteWhereAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) =>
            await _dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.AnyAsync(cancellationToken);

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) =>
            await _dbSet.AnyAsync(predicate, cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await _dbSet.CountAsync(cancellationToken);

    public async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) =>
            await _dbSet.CountAsync(predicate, cancellationToken);
}
