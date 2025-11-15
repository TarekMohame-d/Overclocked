using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Category.Decorators;

public class LoggingCategoryServiceDecorator(
    ICategoryService inner,
    ILogger<LoggingCategoryServiceDecorator> logger)
    : ICategoryService
{
    public Task<Result<CategoryResponse>> GetCategoryByIdAsync(GetCategoryByIdRequest request,
        CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.GetCategoryByIdAsync(request, cancellationToken));

    public Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(GetAllCategoriesRequest request,
        CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.GetAllCategoriesAsync(request, cancellationToken));

    public Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.CreateCategoryAsync(request, cancellationToken));

    public Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.UpdateCategoryAsync(request, cancellationToken));

    public Task<Result> DeleteCategoryAsync(DeleteCategoryRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.DeleteCategoryAsync(request, cancellationToken));

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(
        object request,
        Func<Task<TResult>> action)
        where TResult : Result
    {
        var requestName = request.GetType().Name;
        logger.LogInformation("Processing request {RequestName}", requestName);

        TResult result = await action();

        if (result.IsSuccess)
            logger.LogInformation("Completed request {RequestName}", requestName);
        else
        {
            using (LogContext.PushProperty("Errors", result.Error, true))
                logger.LogError("Completed request {@RequestName} with errors", requestName);
        }

        return result;
    }
}
