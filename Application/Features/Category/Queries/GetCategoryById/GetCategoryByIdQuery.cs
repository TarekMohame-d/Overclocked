using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;

namespace Application.Features.Category.Queries.GetCategoryById;

public record GetCategoryByIdQuery : ICachedQuery<Result<CategoryDto>>
{
    public Guid Id { get; init; }
    public string CacheKey => CacheKeys.Category(Id.ToString());
    public bool BypassCache => false;
}
