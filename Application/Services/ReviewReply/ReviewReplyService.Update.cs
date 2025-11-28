using System.Net;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.ReviewReply.DTOs.Request;
using Application.Services.ReviewReply.Mapping;

namespace Application.Services.ReviewReply;

public sealed partial class ReviewReplyService
{
    public async Task<Result> UpdateReviewReplyAsync(
        UpdateReviewReplyRequest request,
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
            asNoTracking: false,
            cancellationToken: cancellationToken);

        if(reviewReply is null)
            return Result.Failure(Errors.ReviewReplyNotFound, HttpStatusCode.NotFound);

        reviewReply.UpdateFrom(request);

        await unitOfWork.CompleteAsync(cancellationToken);

        await cacheService.RemoveAsync(CacheKeys.Product(request.ProductId.ToString()), cancellationToken);

        return Result.Success();
    }
}
