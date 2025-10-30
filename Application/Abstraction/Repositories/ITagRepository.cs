using Application.Common.Enums;
using Domain.Entities;

namespace Application.Abstraction.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    IQueryable<Tag> GetTagsQuery(TagSortField sortBy, SortDirection direction);
}
