using Overclocked.Contracts.Tag;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Mapping;

public static class TagMapping
{
    public static TagResponse ToDto(this TagEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    public static IEnumerable<TagPagedResponse> ToDto(this IEnumerable<TagEntity> entities)
    {
        return entities.Select(x => new TagPagedResponse
        {
            Id = x.Id,
            Name = x.Name
        });
    }
}
