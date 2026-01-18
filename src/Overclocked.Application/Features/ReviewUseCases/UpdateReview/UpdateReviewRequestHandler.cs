using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewUseCases.UpdateReview;

public class UpdateReviewRequestHandler(
    IReviewRepository reviewRepository,
    IProductReadRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateReviewRequest>
{
    public async Task<Result> Handle(UpdateReviewRequest request, CancellationToken ct)
    {
        var reviewId = ReviewId.Create(request.ReviewId);
        var userId = UserId.Create(request.UserId);
        var productId = ProductId.Create(request.ProductId);

        if (!await productRepository.ExistsAsync(productId, ct))
            return Result.Failure(ProductErrors.ProductNotFound(request.ProductId));

        Review? review = await reviewRepository.GetAsync(
            x => x.Id == reviewId && x.UserId == userId && x.ProductId == productId,
            ct
        );

        if (review is null)
            return Result.Failure(ReviewErrors.ReviewNotFound(request.ReviewId));

        Result result = review.Update(request.Comment, request.Rating);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
