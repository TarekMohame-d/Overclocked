using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.ReviewUseCases.DeleteReview;

public record DeleteReviewRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid ProductId { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid UserId { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.ReviewSet(ProductId.ToString());
}
