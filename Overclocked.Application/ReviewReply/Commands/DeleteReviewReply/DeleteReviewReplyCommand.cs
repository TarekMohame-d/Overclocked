using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.ReviewReply.Commands.DeleteReviewReply;

public record DeleteReviewReplyCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid ProductId { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid ReplyId { get; init; }
    public required Guid EmployeeId { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.ReviewSet;
}
