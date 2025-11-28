using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Application.Services.ReviewReply.DTOs.Request;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Review.Decorators;

public class LoggingReviewServiceDecorator(IReviewService inner, ILogger<LoggingReviewServiceDecorator> logger)
    : IReviewService
{
    public Task<Result<RatingBreakdownResponse>> GetReviewRatingBreakdownAsync(
    Guid productId,
    CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync("GetReviewRatingBreakdownRequest", () => inner.GetReviewRatingBreakdownAsync(productId, cancellationToken));

    public Task<Result<PagedResult<ReviewResponse>>> GetPagedReviewsAsync(
        GetPagedReviewsRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.GetPagedReviewsAsync(request, cancellationToken));

    public Task<Result<ReviewCreatedResponse>> CreateReviewAsync(
        CreateReviewRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.CreateReviewAsync(request, cancellationToken));

    public Task<Result<ReviewUpdatedResponse>> UpdateReviewAsync(
        UpdateReviewRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.UpdateReviewAsync(request, cancellationToken));

    public Task<Result<ReviewDeletedResponse>> DeleteReviewAsync(
        DeleteReviewRequest request,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.DeleteReviewAsync(request, cancellationToken));

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(object request, Func<Task<TResult>> action)
        where TResult : Result
    {
        var requestName = request as string ?? request.GetType().Name;
        logger.LogInformation("Processing request {RequestName}", requestName);

        TResult result = await action();

        if(result.IsSuccess)
        {
            logger.LogInformation("Completed request {RequestName}", requestName);
        }
        else
        {
            using(LogContext.PushProperty("Errors", result.Error, true))
            {
                logger.LogError("Completed request {@RequestName} with errors", requestName);
            }
        }

        return result;
    }
}
