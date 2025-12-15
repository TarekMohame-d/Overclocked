using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;

namespace Overclocked.Application.ReviewReply.Commands.DeleteReviewReply;

public class DeleteReviewReplyCommandHandler(
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteReviewReplyCommand>
{
    public async Task<Result> Handle(DeleteReviewReplyCommand command, CancellationToken cancellationToken)
    {
        var reviewId = ReviewId.Create(command.ReviewId);
        var employeeId = UserId.Create(command.EmployeeId);
        var productId = ProductId.Create(command.ProductId);
        var reviewReplyId = ReviewReplyId.Create(command.ReplyId);

        ReviewEntity? review = await reviewRepository.GetForUpdateAsync(
            x => x.Id == reviewId && x.ProductId == productId,
            cancellationToken);

        if(review is null)
        {
            return Result.Failure(ReviewErrors.ReviewNotFound(command.ReviewId));
        }

        Result result = review.DeleteReply(reviewReplyId, employeeId);

        if(result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
