using System.Net;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Application.Services.Review.Mapping;
using Application.Services.ReviewReply.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Review;

public sealed partial class ReviewService
{
    public async Task<Result<ReviewUpdatedResponse>> UpdateReviewAsync(
        UpdateReviewRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Review? review = await reviewRepository.SingleOrDefaultAsync(
            x => x.ProductId == request.ProductId && x.UserId == request.UserId && x.Id == request.ReviewId,
            include: q => q.Include(r => r.Product),
            asNoTracking: false,
            cancellationToken);

        if(review is null)
            return Result<ReviewUpdatedResponse>.Failure(Errors.ReviewNotFound, HttpStatusCode.NotFound);

        if(review.Product is null)
            return Result<ReviewUpdatedResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        review.Product.UpdateRating(review.Rating, request.Rating);

        review.UpdateFrom(request);

        await unitOfWork.CompleteAsync(cancellationToken);

        ReviewUpdatedResponse reviewUpdatedResponse = review.ToDto(review.Product.ReviewCount, review.Product.Rating);

        await cacheService.RemoveAsync(CacheKeys.Product(review.ProductId.ToString()), cancellationToken);

        return Result<ReviewUpdatedResponse>.Success(reviewUpdatedResponse);
    }
}
