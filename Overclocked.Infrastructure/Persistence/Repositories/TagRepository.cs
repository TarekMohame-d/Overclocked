using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class TagRepository(ApplicationDbContext context)
    : GenericRepository<Tag, TagId>(context), ITagRepository
{
    public Task<int> CountAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> query = _dbSet.AsNoTracking();
        if(!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedTerm = searchTerm.ToUpper();
            query = query.Where(t => t.NormalizedName.Contains(normalizedTerm));
        }

        return query.CountAsync(cancellationToken);
    }

    public Task<List<Tag>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        TagSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Tag> query = _dbSet.AsNoTracking();

        if(!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedTerm = searchTerm.ToUpper();
            query = query.Where(t => t.NormalizedName.Contains(normalizedTerm));
        }

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(cancellationToken);
    }

    public Task<List<Tag>> WhereAsync(
        Expression<Func<Tag, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
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

    public Task<Tag?> FindAsync(TagId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync([id], cancellationToken: cancellationToken).AsTask();
    }
    public Task<Tag?> GetByIdAsync(TagId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }
}
