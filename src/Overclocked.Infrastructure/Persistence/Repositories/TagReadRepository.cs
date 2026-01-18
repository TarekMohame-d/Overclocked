using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.TagAggregate;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class TagReadRepository(ApplicationDbContext dbContext) : ITagReadRepository
{
    private readonly IQueryable<Tag> _queryable = dbContext.Tags.AsNoTracking();

    public Task<int> CountAsync(string searchTerm, CancellationToken ct = default)
    {
        IQueryable<Tag> query = _queryable;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedTerm = searchTerm.ToUpper();
            query = query.Where(t => t.NormalizedName.Contains(normalizedTerm));
        }

        return query.CountAsync(ct);
    }

    public Task<List<Tag>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        TagSortField sortBy,
        SortDirection direction,
        CancellationToken ct = default
    )
    {
        IQueryable<Tag> query = _queryable;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedTerm = searchTerm.ToUpperInvariant();
            query = query.Where(t => t.NormalizedName.Contains(normalizedTerm));
        }

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(ct);
    }

    private static IQueryable<Tag> ApplySorting(IQueryable<Tag> query, TagSortField sortBy, SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            TagSortField.Name => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),

            TagSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }

    public Task<List<Guid>> GetExistingTagIdsAsync(List<Guid> candidateTagIds, CancellationToken ct = default) =>
        _queryable.Where(x => candidateTagIds.Contains(x.Id)).Select(x => x.Id.Value).ToListAsync(ct);
}
