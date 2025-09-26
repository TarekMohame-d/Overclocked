using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;

namespace Application.Features.Tag.Queries.GetTagById;

public record GetTagByIdQuery : ICachedQuery<Result<TagDto>>
{
    public Guid Id { get; init; }
    public string CacheKey => CacheKeys.Tag(Id.ToString());
    public string? CacheSetKey => null;
    public bool BypassCache => false;
}
