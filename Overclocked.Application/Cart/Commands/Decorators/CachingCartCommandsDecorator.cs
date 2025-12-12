using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Cart.Commands.Decorators;

public class CachingCartCommandsDecorator(
    ICartCommands inner,
    ICacheService cacheService,
    ILogger<CachingCartCommandsDecorator> logger) : ICartCommands
{
    public async Task<Result<CartItemResponse>> AddCartItemCommandHandler(AddCartItemCommand command, CancellationToken cancellationToken)
    {
        Result<CartItemResponse> result = await inner.AddCartItemCommandHandler(command, cancellationToken);

        if(result.IsSuccess)
        {
            logger.LogInformation(
                "Removing cache for Cart key: {CacheKey}",
                CacheKeys.Cart(result.Value!.CartId.ToString()));

            await cacheService.RemoveAsync(CacheKeys.Cart(result.Value!.CartId.ToString()), cancellationToken);

            logger.LogInformation(
                "Removed cache for Cart key: {CacheKey}",
                CacheKeys.Cart(result.Value!.CartId.ToString()));
        }

        return result;
    }
}
