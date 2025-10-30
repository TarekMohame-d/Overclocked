using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Tag.DTOs.Request;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Brand.Decorators;

public class LoggingBrandServiceDecorator : IBrandService
{
    private readonly IBrandService _inner;
    private readonly ILogger<LoggingBrandServiceDecorator> _logger;

    public LoggingBrandServiceDecorator(
        IBrandService inner,
        ILogger<LoggingBrandServiceDecorator> logger)
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

    public Task<Result<BrandResponse>> GetBrandByIdAsync(GetBrandByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.GetBrandByIdAsync(request, cancellationToken));

    public Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(GetAllBrandsRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.GetAllBrandsAsync(request, cancellationToken));

    public Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.CreateBrandAsync(request, cancellationToken));

    public Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.UpdateBrandAsync(request, cancellationToken));

    public Task<Result> DeleteBrandAsync(DeleteBrandRequest request, CancellationToken cancellationToken)
        => ExecuteWithLoggingAsync(request, () => _inner.DeleteBrandAsync(request, cancellationToken));
}
