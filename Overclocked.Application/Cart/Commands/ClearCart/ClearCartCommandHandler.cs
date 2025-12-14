using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.ValueObjects;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;

namespace Overclocked.Application.Cart.Commands.ClearCart;

public class ClearCartCommandHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    : ICommandHandler<ClearCartCommand>
{
    public async Task<Result> Handle(ClearCartCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);

        CartEntity cart = await cartRepository.GetCartAsync(userId, cancellationToken)
            ?? throw new CartNotFoundException(command.UserId);

        cart.ClearCart();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
