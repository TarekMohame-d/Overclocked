using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Tag.Decorators;

public class LoggingTagServiceDecorator : ITagService
{
    private readonly ITagService _inner;
    private readonly ILogger<LoggingTagServiceDecorator> _logger;

    public LoggingTagServiceDecorator(
        ITagService inner,
        ILogger<LoggingTagServiceDecorator> logger)
    {
        _inner = inner;
        _logger = logger;
    }

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(
    object request,
    Func<Task<TResult>> action)
    where TResult : Result
    {
        string requestName = request.GetType().Name;
        _logger.LogInformation("Processing request {RequestName}", requestName);

        var result = await action();

        if (result.IsSuccess)
        {
            _logger.LogInformation("Completed request {RequestName}", requestName);
        }
        else
        {
            using (LogContext.PushProperty("Errors", result.Error, true))
            {
                _logger.LogError("Completed request {@RequestName} with errors", requestName);
            }
        }

        return result;
    }

    public Task<Result<TagResponse>> GetTagByIdAsync(GetTagByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.GetTagByIdAsync(request, cancellationToken));

    public Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(GetPagedTagsQuery query, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(query, () => _inner.GetPagedTagsAsync(query, cancellationToken));

    public Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.CreateTagAsync(request, cancellationToken));

    public Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.UpdateTagAsync(request, cancellationToken));

    public Task<Result> DeleteTagAsync(DeleteTagRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.DeleteTagAsync(request, cancellationToken));
}
