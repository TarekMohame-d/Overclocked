using Application.Common.Results;
using Application.Services.ReviewReply.DTOs.Request;

namespace Application.Abstraction.DomainServices;

public interface IReviewReplyService
{
    Task<Result> CreateReviewReplyAsync(CreateReviewReplyRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateReviewReplyAsync(UpdateReviewReplyRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteReviewReplyAsync(DeleteReviewReplyRequest request, CancellationToken cancellationToken);
}
