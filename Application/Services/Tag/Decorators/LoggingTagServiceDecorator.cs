using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Tag.Decorators;

public class LoggingTagServiceDecorator(
    ITagService inner,
    ILogger<LoggingTagServiceDecorator> logger)
    : ITagService
{
    public Task<Result<TagResponse>> GetTagByIdAsync(GetTagByIdRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.GetTagByIdAsync(request, cancellationToken));

    public Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(
        GetPagedTagsRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.GetPagedTagsAsync(request, cancellationToken));

    public Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.CreateTagAsync(request, cancellationToken));

    public Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.UpdateTagAsync(request, cancellationToken));

    public Task<Result> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync("DeleteTagRequest", () => inner.DeleteTagAsync(tagId, cancellationToken));

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
