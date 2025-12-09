using Overclocked.Contracts.Tag;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Mapping;

public static class TagMapping
{
    public static TagResponse ToDto(this TagEntity entity)
    {
        return new(entity.Id, entity.Name);
    }

    public static IEnumerable<TagListResponse> ToDto(this IEnumerable<TagEntity> entities)
    {
        return entities.Select(x => new TagListResponse(x.Id, x.Name));
    }
}
