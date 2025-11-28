using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Cart.DTOs.Request;
using Application.Services.Cart.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Cart.Decorators;

public class LoggingCartServiceDecorator(ICartService inner, ILogger<LoggingCartServiceDecorator> logger) : ICartService
{
    public Task<Result> CreateCartAsync(Guid userId, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync("CreateCart", () => inner.CreateCartAsync(userId, cancellationToken));

    public Task<Result<CartItemResponse>> GetCartItemsAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync("GetCartItemsRequest", () => inner.GetCartItemsAsync(userId, cancellationToken));

    public Task<Result> AddCartItemAsync(
        AddCartItemRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.AddCartItemAsync(request, cancellationToken));

    public Task<Result> UpdateCartItemAsync(
        UpdateCartItemRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.UpdateCartItemAsync(request, cancellationToken));

    public Task<Result> DeleteCartItemAsync(DeleteCartItemRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.DeleteCartItemAsync(request, cancellationToken));

    public Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync("ClearCartRequest", () => inner.ClearCartAsync(userId, cancellationToken));

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
