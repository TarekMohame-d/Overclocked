using System.Net;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Application.Services.Review.Mapping;
using Application.Services.ReviewReply.Mapping;

namespace Application.Services.Review;

public sealed partial class ReviewService
{
    public async Task<Result<ReviewCreatedResponse>> CreateReviewAsync(
        CreateReviewRequest request,
        CancellationToken cancellationToken)
    {
        var reviewExist = await reviewRepository.AnyAsync(
            x => x.ProductId == request.ProductId && x.UserId == request.UserId,
            cancellationToken);

        if(reviewExist)
            return Result<ReviewCreatedResponse>.Failure(Errors.AlreadyReviewedThisProduct, HttpStatusCode.Conflict);

        Domain.Entities.Product? product = await productRepository.SingleOrDefaultAsync(
            x => x.Id == request.ProductId,
            asNoTracking: false,
            cancellationToken);

        if(product is null)
            return Result<ReviewCreatedResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        Domain.Entities.Review review = request.ToEntity();

        product.CalculateRating(request.Rating);

        await reviewRepository.AddAsync(review, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        ReviewCreatedResponse reviewCreatedResponse = review.ToDto(product.Rating, product.ReviewCount);

        await cacheService.RemoveAsync(CacheKeys.Product(product.Id.ToString()), cancellationToken);

        return Result<ReviewCreatedResponse>.Success(reviewCreatedResponse, HttpStatusCode.Created);
    }
}
