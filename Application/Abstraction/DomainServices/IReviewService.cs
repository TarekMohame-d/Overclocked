using Application.Common.Results;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Application.Services.ReviewReply.DTOs.Request;

namespace Application.Abstraction.DomainServices;

public interface IReviewService
{
    Task<Result<RatingBreakdownResponse>> GetReviewRatingBreakdownAsync(
        Guid productId,
        CancellationToken cancellationToken);
    Task<Result<PagedResult<ReviewResponse>>> GetPagedReviewsAsync(
        GetPagedReviewsRequest request,
        CancellationToken cancellationToken);
    Task<Result<ReviewCreatedResponse>> CreateReviewAsync(
        CreateReviewRequest request,
        CancellationToken cancellationToken);
    Task<Result<ReviewUpdatedResponse>> UpdateReviewAsync(
        UpdateReviewRequest request,
        CancellationToken cancellationToken);
    Task<Result<ReviewDeletedResponse>> DeleteReviewAsync(
        DeleteReviewRequest request,
        CancellationToken cancellationToken);
}
