using System.Linq.Expressions;
using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IGenericRepository<T, TId>
    where T : AggregateRoot<TId>
    where TId : IEntityKey
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);

    Task<int> DeleteWhereAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
}
