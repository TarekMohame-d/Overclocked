using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.CartUseCases.ClearCart;

public class ClearCartRequestHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork) : IRequestHandler<ClearCartRequest>
{
    public async Task<Result> Handle(ClearCartRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);

        Cart cart = await cartRepository.GetAsync(userId, ct) ?? throw new CartNotFoundException(request.UserId);

        cart.Clear();

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
