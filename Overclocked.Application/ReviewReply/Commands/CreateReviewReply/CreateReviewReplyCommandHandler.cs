using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;
using ReviewReplyEntity = Overclocked.Domain.ReviewAggregate.Entities.ReviewReply;

namespace Overclocked.Application.ReviewReply.Commands.CreateReviewReply;

public class CreateReviewReplyCommandHandler(
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateReviewReplyCommand>
{
    public async Task<Result> Handle(CreateReviewReplyCommand command, CancellationToken cancellationToken)
    {
        var reviewId = ReviewId.Create(command.ReviewId);
        var employeeId = UserId.Create(command.EmployeeId);
        var productId = ProductId.Create(command.ProductId);

        ReviewEntity? review = await reviewRepository.GetForUpdateAsync(
            x => x.Id == reviewId && x.ProductId == productId,
            cancellationToken);

        if(review is null)
        {
            return Result.Failure(ReviewErrors.ReviewNotFound(command.ReviewId));
        }

        if(review.ReviewReply is not null)
        {
            return Result.Failure(ReviewErrors.ReviewAlreadyHasReply);
        }

        var reviewReply = ReviewReplyEntity.Create(
            id: ReviewReplyId.Create(),
            employeeId: employeeId,
            reply: command.Reply);

        review.AddReviewReply(reviewReply);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
