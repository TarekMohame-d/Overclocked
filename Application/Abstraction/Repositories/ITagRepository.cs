using Domain.Entities;

namespace Application.Abstraction.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    IQueryable<Tag> GetTagsQuery(string? sortBy);
}
