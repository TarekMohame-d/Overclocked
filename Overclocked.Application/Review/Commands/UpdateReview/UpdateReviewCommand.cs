using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Review.Commands.UpdateReview;

public record UpdateReviewCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid ProductId { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid UserId { get; init; }
    public required int Rating { get; init; }
    public required string Comment { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.ReviewSet;
}
