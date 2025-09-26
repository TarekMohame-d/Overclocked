using Domain.Entities;

namespace Domain.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    IQueryable<Tag> GetTagsQuery(string? sortBy);
}
