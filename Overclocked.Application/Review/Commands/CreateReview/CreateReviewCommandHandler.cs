using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Mapping;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Events;
using Overclocked.Domain.UserAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;

namespace Overclocked.Application.Review.Commands.CreateReview;

public class CreateReviewCommandHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateReviewCommand>
{
    public async Task<Result> Handle(
        CreateReviewCommand command,
        CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(command.ProductId);
        var userId = UserId.Create(command.UserId);

        var reviewExist = await reviewRepository.AnyAsync(
            x => x.ProductId == productId && x.UserId == userId,
            cancellationToken);

        if(reviewExist)
        {
            return Result.Failure(ReviewErrors.AlreadyReviewedThisProduct);
        }

        ReviewEntity review = command.ToEntity();

        await reviewRepository.AddAsync(review, cancellationToken);

        review.RaiseDomainEvent(new ReviewCreatedEvent(command.ProductId, command.Rating));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
