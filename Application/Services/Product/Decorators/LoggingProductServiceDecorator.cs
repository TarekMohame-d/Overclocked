using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Product.Decorators;

public class LoggingProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly ILogger<LoggingProductServiceDecorator> _logger;

    public LoggingProductServiceDecorator(
        IProductService inner,
        ILogger<LoggingProductServiceDecorator> logger)
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

    public Task<Result<ProductResponse>> GetProductByIdAsync(GetProductByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.GetProductByIdAsync(request, cancellationToken));

    public Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(GetPagedProductsQuery query, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(query, () => _inner.GetPagedProductsAsync(query, cancellationToken));

    public Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.CreateProductAsync(request, cancellationToken));

    public Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.UpdateProductAsync(request, cancellationToken));

    public Task<Result> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.DeleteProductAsync(request, cancellationToken));
}
