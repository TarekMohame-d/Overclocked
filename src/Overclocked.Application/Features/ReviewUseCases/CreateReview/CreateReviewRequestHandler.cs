using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewUseCases.CreateReview;

public class CreateReviewRequestHandler(
    IReviewRepository reviewRepository,
    IProductReadRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateReviewRequest>
{
    public async Task<Result> Handle(CreateReviewRequest request, CancellationToken ct)
    {
        var productId = ProductId.Create(request.ProductId);
        var userId = UserId.Create(request.UserId);

        if (!await productRepository.ExistsAsync(productId, ct))
            return Result.Failure(ProductErrors.ProductNotFound(request.ProductId));

        var reviewExist = await reviewRepository.ExistsAsync(x => x.ProductId == productId && x.UserId == userId, ct);

        if (reviewExist)
            return Result.Failure(ReviewErrors.AlreadyReviewedThisProduct);

        Result<Review> reviewResult = Review.Create(
            UserId.Create(request.UserId),
            ProductId.Create(request.ProductId),
            request.Comment,
            request.Rating
        );

        if (reviewResult.IsFailure)
            return Result.Failure(reviewResult.Error);

        Review review = reviewResult.Value;

        reviewRepository.Add(review);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
