using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class TagRepository(ApplicationDbContext context)
    : GenericRepository<Tag, TagId>(context), ITagRepository
{
    private readonly ApplicationDbContext _dbContext = context;

    public Task<int> CountAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var normalizedTerm = searchTerm.ToUpper();

        IQueryable<Tag> query = _dbContext.Tags.Where(t => t.NormalizedName.Contains(normalizedTerm));

        return query.CountAsync(cancellationToken);
    }

    public Task<List<Tag>> GetTagsAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        TagSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default)
    {
        var normalizedTerm = searchTerm.ToUpper();

        IQueryable<Tag> query = _dbContext.Tags.AsNoTracking();

        query = query.Where(t => t.NormalizedName.Contains(normalizedTerm));

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(cancellationToken);
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
}
