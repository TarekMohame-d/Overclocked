using System.Linq.Expressions;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    private readonly ApplicationDbContext _context;

    public TagRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public IQueryable<Tag> GetTagsQuery(string? sortBy)
    {
        IQueryable<Tag> query = _context.Tags.AsQueryable();

        query = sortBy switch
        {
            "name_asc" => query.OrderBy(t => t.Name),
            "name_desc" => query.OrderByDescending(t => t.Name),
            null => query.OrderBy(t => t.Id),
            _ => query,
        };

        return query;
    }
}
