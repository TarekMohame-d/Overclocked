using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Wishlist.DTOs.Request;
using Application.Services.Wishlist.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Wishlist.Decorators;

public class LoggingWishlistServiceDecorator(
    IWishlistService inner,
    ILogger<LoggingWishlistServiceDecorator> logger)
    : IWishlistService
{
    public Task<Result> CreateWishlistAsync(Guid userId, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync("CreateWishlist", () => inner.CreateWishlistAsync(userId, cancellationToken));

    public Task<Result<IEnumerable<WishlistItemResponse>>> GetWishlistItemsAsync(
        Guid userId,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync("GetWishlistItems", () => inner.GetWishlistItemsAsync(userId, cancellationToken));

    public Task<Result> AddWishlistItemAsync(AddWishlistItemRequest request, CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(request, () => inner.AddWishlistItemAsync(request, cancellationToken));

    public Task<Result> DeleteWishlistItemAsync(
        DeleteWishlistItemRequest request,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.DeleteWishlistItemAsync(request, cancellationToken));

    public Task<Result> ClearWishlistAsync(Guid userId, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync("ClearWishlist", () => inner.ClearWishlistAsync(userId, cancellationToken));

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
