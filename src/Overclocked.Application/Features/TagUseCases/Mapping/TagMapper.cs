using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.Domain.TagAggregate;

namespace Overclocked.Application.Features.TagUseCases.Mapping;

public static class TagMapper
{
    public static IEnumerable<TagPagedResponse> ToDto(this IEnumerable<Tag> entities) =>
        entities.Select(x => new TagPagedResponse { Id = x.Id, Name = x.Name });
}
