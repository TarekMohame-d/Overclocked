using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Cart.Commands;

public interface ICartCommands
{
    Task<Result<CartItemResponse>> AddCartItemCommandHandler(
        AddCartItemCommand command,
        CancellationToken cancellationToken);
}
