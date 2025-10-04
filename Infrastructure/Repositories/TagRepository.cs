using Application.Abstraction.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        IQueryable<Tag> query = _context.Tags.AsNoTracking();

        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderBy(p => p.Id);

        var parts = sortBy.Split('_');
        if (parts.Length != 2)
            return query.OrderBy(p => p.Id);

        var field = parts[0].ToLowerInvariant();
        var direction = parts[1].ToLowerInvariant();
        bool isDescending = direction == "desc";

        switch (field)
        {
            case "name":
                query = isDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name);
                break;

            default:
                query = query.OrderBy(p => p.Id);
                break;
        }

        return query;
    }
}
