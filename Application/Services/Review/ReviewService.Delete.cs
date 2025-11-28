using System.Net;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Review;

public sealed partial class ReviewService
{
    public async Task<Result<ReviewDeletedResponse>> DeleteReviewAsync(
        DeleteReviewRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Review? review = await reviewRepository.SingleOrDefaultAsync(
            x => x.Id == request.ReviewId && x.ProductId == request.ProductId && x.UserId == request.UserId,
            include: q => q.Include(r => r.Product),
            asNoTracking: false,
            cancellationToken);

        if(review is null)
            return Result<ReviewDeletedResponse>.Failure(Errors.ReviewNotFound, HttpStatusCode.NotFound);

        if(review.Product is null)
            return Result<ReviewDeletedResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        reviewRepository.Delete(review);

        review.Product.RemoveRating(review.Rating);

        await unitOfWork.CompleteAsync(cancellationToken);

        var reviewDeletedResponse = new ReviewDeletedResponse
        {
            AverageRating = review.Product.Rating,
            ReviewCount = review.Product.ReviewCount
        };

        await cacheService.RemoveAsync(CacheKeys.Product(review.ProductId.ToString()), cancellationToken);

        return Result<ReviewDeletedResponse>.Success(reviewDeletedResponse);
    }
}
