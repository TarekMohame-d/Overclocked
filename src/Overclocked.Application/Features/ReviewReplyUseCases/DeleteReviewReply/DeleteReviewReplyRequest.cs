using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.ReviewReplyUseCases.DeleteReviewReply;

public record DeleteReviewReplyRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid ProductId { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid ReplyId { get; init; }
    public required Guid EmployeeId { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.ReviewSet(ProductId.ToString());
}
