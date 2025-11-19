using Application.Abstraction.Repositories;
using Application.Common.Enums;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public IQueryable<Tag> GetTagsQuery(TagSortField sortBy, SortDirection direction)
    {
        IQueryable<Tag> query = _context.Tags.AsNoTracking();

        query = ApplySorting(query, sortBy, direction);

        return query;
    }

    private IQueryable<Tag> ApplySorting(IQueryable<Tag> query, TagSortField sortBy, SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            TagSortField.Name => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),

            TagSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }
}
