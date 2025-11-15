using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Product.Decorators;

public class LoggingProductServiceDecorator(
    IProductService inner,
    ILogger<LoggingProductServiceDecorator> logger)
    : IProductService
{
    public Task<Result<ProductResponse>> GetProductByIdAsync(GetProductByIdRequest request,
        CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.GetProductByIdAsync(request, cancellationToken));

    public Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(GetPagedProductsRequest request,
        CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.GetPagedProductsAsync(request, cancellationToken));

    public Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.CreateProductAsync(request, cancellationToken));

    public Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.UpdateProductAsync(request, cancellationToken));

    public Task<Result> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => inner.DeleteProductAsync(request, cancellationToken));

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
