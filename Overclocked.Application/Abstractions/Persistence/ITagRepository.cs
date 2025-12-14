using System.Linq.Expressions;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ITagRepository : IGenericRepository<TagEntity, TagId>
{
    Task<TagEntity?> FindAsync(TagId id, CancellationToken cancellationToken = default);
    Task<TagEntity?> GetByIdAsync(
        TagId id,
        CancellationToken cancellationToken = default);
    Task<int> CountAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<TagEntity>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        TagSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default);
    Task<List<TagEntity>> WhereAsync(
        Expression<Func<TagEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
}
