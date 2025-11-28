using System.Net;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.ReviewReply.DTOs.Request;

namespace Application.Services.ReviewReply;

public sealed partial class ReviewReplyService
{
    public async Task<Result> DeleteReviewReplyAsync(
        DeleteReviewReplyRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Review? review = await reviewRepository
            .SingleOrDefaultAsync(
            x => x.Id == request.ReviewId && x.ProductId == request.ProductId,
            cancellationToken: cancellationToken);

        if(review is null)
            return Result.Failure(Errors.ReviewNotFound, HttpStatusCode.NotFound);

        Domain.Entities.ReviewReply? reviewReply = await reviewReplyRepository
            .SingleOrDefaultAsync(
            x => x.Id == request.ReplyId && x.ReviewId == request.ReviewId,
            cancellationToken: cancellationToken);

        if(reviewReply is null)
            return Result.Failure(Errors.ReviewReplyNotFound, HttpStatusCode.NotFound);

        reviewReplyRepository.Delete(reviewReply);

        await unitOfWork.CompleteAsync(cancellationToken);

        await cacheService.RemoveAsync(CacheKeys.Product(request.ProductId.ToString()), cancellationToken);

        return Result.Success();
    }
}
