using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;

namespace Application.Features.Category.Queries.GetCategoryById;

public record GetCategoryByIdQuery : ICachedRequest<Result<CategoryDto>>
{
    public Guid Id { get; init; }
    public string CacheKey => CacheKeys.Category(Id.ToString());
    public string? CacheSetKey => null;
    public bool BypassCache => false;
}
