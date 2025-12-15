using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.ReviewReply.Commands.CreateReviewReply;

public record CreateReviewReplyCommand : ICommand, ICacheInvalidatorCommand
{
    public required string Reply { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required Guid ProductId { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.ReviewSet;
}
