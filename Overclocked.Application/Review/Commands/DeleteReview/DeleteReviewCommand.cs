using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Review.Commands.DeleteReview;

public record DeleteReviewCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid ProductId { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid UserId { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.ReviewSet;
}
