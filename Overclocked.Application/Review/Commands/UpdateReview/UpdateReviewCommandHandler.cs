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

namespace Overclocked.Application.Review.Commands.UpdateReview;

public class UpdateReviewCommandHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateReviewCommand>
{
    public async Task<Result> Handle(UpdateReviewCommand command, CancellationToken cancellationToken)
    {
        var reviewId = ReviewId.Create(command.ReviewId);
        var userId = UserId.Create(command.UserId);
        var productId = ProductId.Create(command.ProductId);

        ReviewEntity? review = await reviewRepository.GetForUpdateAsync(
            x => x.Id == reviewId && x.UserId == userId && x.ProductId == productId,
            cancellationToken);

        if(review is null)
        {
            return Result.Failure(ReviewErrors.ReviewNotFound(command.ReviewId));
        }

        var oldRating = review.Rating;
        var newRating = command.Rating;

        review.Update(command.Comment, command.Rating);

        review.RaiseDomainEvent(new ReviewUpdatedEvent(productId.Value, oldRating, newRating));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
