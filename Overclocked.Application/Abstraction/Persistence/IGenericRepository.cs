using System.Linq.Expressions;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : IEntityKey
{
    Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = true, CancellationToken cancellationToken = default);
    IQueryable<T> Query(bool asNoTracking = true);
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<T?> SingleOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> WhereAsync(
        Expression<Func<T, bool>> predicate,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> WhereAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IQueryable<T>>? include,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
