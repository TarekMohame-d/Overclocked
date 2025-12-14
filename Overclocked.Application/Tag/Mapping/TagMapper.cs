using Overclocked.Contracts.Tag;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Mapping;

public static class TagMapper
{
    public static IEnumerable<TagPagedResponse> ToDto(this IEnumerable<TagEntity> entities)
    {
        return entities.Select(x => new TagPagedResponse
        {
            Id = x.Id,
            Name = x.Name
        });
    }
}
