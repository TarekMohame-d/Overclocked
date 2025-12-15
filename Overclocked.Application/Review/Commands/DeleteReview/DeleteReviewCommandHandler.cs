using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Events;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;

namespace Overclocked.Application.Review.Commands.DeleteReview;

public class DeleteReviewCommandHandler(
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteReviewCommand>
{
    public async Task<Result> Handle(DeleteReviewCommand command, CancellationToken cancellationToken)
    {
        var reviewId = ReviewId.Create(command.ReviewId);
        var userId = UserId.Create(command.UserId);
        var productId = ProductId.Create(command.ProductId);

        ReviewEntity? review = await reviewRepository.GetById(
            x => x.Id == reviewId && x.UserId == userId && x.ProductId == productId,
            cancellationToken);

        if(review is null)
        {
            return Result.Failure(ReviewErrors.ReviewNotFound(command.ReviewId));
        }

        reviewRepository.Delete(review);

        review.RaiseDomainEvent(new ReviewDeletedEvent(productId.Value, review.Rating));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
