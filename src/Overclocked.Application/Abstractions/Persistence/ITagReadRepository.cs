using Overclocked.Application.Common.Enums;
using Overclocked.Domain.TagAggregate;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ITagReadRepository : IRepository
{
    Task<int> CountAsync(string searchTerm, CancellationToken ct = default);

    Task<List<Tag>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        TagSortField sortBy,
        SortDirection direction,
        CancellationToken ct = default
    );

    Task<List<Guid>> GetExistingTagIdsAsync(List<Guid> candidateTagIds, CancellationToken ct = default);
}
