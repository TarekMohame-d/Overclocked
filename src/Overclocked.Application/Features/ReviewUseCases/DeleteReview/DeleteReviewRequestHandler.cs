using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewUseCases.DeleteReview;

public class DeleteReviewRequestHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteReviewRequest>
{
    public async Task<Result> Handle(DeleteReviewRequest request, CancellationToken ct)
    {
        var reviewId = ReviewId.Create(request.ReviewId);
        var userId = UserId.Create(request.UserId);
        var productId = ProductId.Create(request.ProductId);

        Review? review = await reviewRepository.GetAsync(
            x => x.Id == reviewId && x.UserId == userId && x.ProductId == productId,
            ct
        );

        if (review is null)
            return Result.Failure(ReviewErrors.ReviewNotFound(request.ReviewId));

        review.Delete(productId.Value, review.Rating);

        reviewRepository.Remove(review);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
