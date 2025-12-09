using Overclocked.Application.Common.Enums;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Abstraction.Persistence;

public interface ITagRepository : IGenericRepository<TagEntity, TagId>
{
    Task<int> CountAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<TagEntity>> GetTagsAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        TagSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default);
}
