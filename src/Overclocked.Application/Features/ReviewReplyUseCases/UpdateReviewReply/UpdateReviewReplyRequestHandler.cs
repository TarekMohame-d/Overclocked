using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewReplyUseCases.UpdateReviewReply;

public class UpdateReviewReplyRequestHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateReviewReplyRequest>
{
    public async Task<Result> Handle(UpdateReviewReplyRequest request, CancellationToken ct)
    {
        var reviewId = ReviewId.Create(request.ReviewId);
        var employeeId = UserId.Create(request.EmployeeId);
        var productId = ProductId.Create(request.ProductId);
        var reviewReplyId = ReviewReplyId.Create(request.ReplyId);

        Review? review = await reviewRepository.GetAsync(x => x.Id == reviewId && x.ProductId == productId, ct);

        if (review is null)
            return Result.Failure(ReviewErrors.ReviewNotFound(request.ReviewId));

        Result result = review.UpdateReply(reviewReplyId, employeeId, request.Reply);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
