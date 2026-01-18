using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewReplyUseCases.CreateReviewReply;

public class CreateReviewReplyRequestHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateReviewReplyRequest>
{
    public async Task<Result> Handle(CreateReviewReplyRequest request, CancellationToken ct)
    {
        var reviewId = ReviewId.Create(request.ReviewId);
        var employeeId = UserId.Create(request.EmployeeId);
        var productId = ProductId.Create(request.ProductId);

        Review? review = await reviewRepository.GetAsync(x => x.Id == reviewId && x.ProductId == productId, ct);

        if (review is null)
            return Result.Failure(ReviewErrors.ReviewNotFound(request.ReviewId));

        if (review.ReviewReply is not null)
            return Result.Failure(ReviewErrors.ReviewAlreadyHasReply);

        Result<ReviewReply> reviewReplyResult = ReviewReply.Create(employeeId, request.Reply);

        if (reviewReplyResult.IsFailure)
            return Result.Failure(reviewReplyResult.Error);

        ReviewReply reviewReply = reviewReplyResult.Value;

        review.AddReviewReply(reviewReply);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
