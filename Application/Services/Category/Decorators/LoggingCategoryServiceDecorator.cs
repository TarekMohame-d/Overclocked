using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Category.Decorators;

public class LoggingCategoryServiceDecorator : ICategoryService
{
    private readonly ICategoryService _inner;
    private readonly ILogger<LoggingCategoryServiceDecorator> _logger;

    public LoggingCategoryServiceDecorator(
        ICategoryService inner,
        ILogger<LoggingCategoryServiceDecorator> logger)
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

    public Task<Result<CategoryResponse>> GetCategoryByIdAsync(GetCategoryByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.GetCategoryByIdAsync(request, cancellationToken));

    public Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(GetAllCategoriesRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.GetAllCategoriesAsync(request, cancellationToken));

    public Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.CreateCategoryAsync(request, cancellationToken));

    public Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.UpdateCategoryAsync(request, cancellationToken));

    public Task<Result> DeleteCategoryAsync(DeleteCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.DeleteCategoryAsync(request, cancellationToken));
}
