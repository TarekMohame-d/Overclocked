using System.Net;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.ReviewReply.DTOs.Request;
using Application.Services.ReviewReply.Mapping;

namespace Application.Services.ReviewReply;

public sealed partial class ReviewReplyService
{
    public async Task<Result> CreateReviewReplyAsync(
        CreateReviewReplyRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Review? review = await reviewRepository
            .SingleOrDefaultAsync(
            x => x.Id == request.ReviewId && x.ProductId == request.ProductId,
            cancellationToken: cancellationToken);

        if(review is null)
            return Result.Failure(Errors.ReviewNotFound, HttpStatusCode.NotFound);

        var replyExist = await reviewReplyRepository
            .AnyAsync(x => x.ReviewId == request.ReviewId, cancellationToken);

        if(replyExist)
            return Result.Failure(Errors.ReviewAlreadyHasReply, HttpStatusCode.Conflict);

        Domain.Entities.ReviewReply reviewReply = request.ToEntity();

        await reviewReplyRepository.AddAsync(reviewReply, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        await cacheService.RemoveAsync(CacheKeys.Product(review.ProductId.ToString()), cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
