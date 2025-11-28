using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.ReviewReply.DTOs.Request;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.ReviewReply.Decorators;

public class LoggingReviewReplyServiceDecorator(
    IReviewReplyService inner,
    ILogger<LoggingReviewReplyServiceDecorator> logger)
    : IReviewReplyService
{
    public Task<Result> CreateReviewReplyAsync(
    CreateReviewReplyRequest request,
    CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.CreateReviewReplyAsync(request, cancellationToken));

    public Task<Result> UpdateReviewReplyAsync(
        UpdateReviewReplyRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.UpdateReviewReplyAsync(request, cancellationToken));

    public Task<Result> DeleteReviewReplyAsync(
        DeleteReviewReplyRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () =>
                inner.DeleteReviewReplyAsync(request, cancellationToken));

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
