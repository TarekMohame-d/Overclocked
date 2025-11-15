using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Brand.Decorators;

public class LoggingBrandServiceDecorator(
    IBrandService inner,
    ILogger<LoggingBrandServiceDecorator> logger)
    : IBrandService
{
    public Task<Result<BrandResponse>> GetBrandByIdAsync(GetBrandByIdRequest request,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.GetBrandByIdAsync(request, cancellationToken));

    public Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(GetAllBrandsRequest request,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.GetAllBrandsAsync(request, cancellationToken));

    public Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.CreateBrandAsync(request, cancellationToken));

    public Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.UpdateBrandAsync(request, cancellationToken));

    public Task<Result> DeleteBrandAsync(DeleteBrandRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.DeleteBrandAsync(request, cancellationToken));

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
