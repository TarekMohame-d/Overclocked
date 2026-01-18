using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.AuthenticationUseCases.EventHandlers;

public class UserEmailConfirmedDomainEventHandler(
    ICartRepository cartRepository,
    IWishlistRepository wishlistRepository,
    IUnitOfWork unitOfWork
) : IDomainEventHandler<UserEmailConfirmedEvent>
{
    public async Task Handle(UserEmailConfirmedEvent domainEvent, CancellationToken ct = default)
    {
        var userId = UserId.Create(domainEvent.UserId);

        var existingCart = await cartRepository.ExistsAsync(userId, ct);
        if (!existingCart)
        {
            var cart = Cart.Create(userId);
            cartRepository.Add(cart);
        }

        var existingWishlist = await wishlistRepository.ExistsAsync(userId, ct);
        if (existingWishlist)
            return;

        var wishlist = Wishlist.Create(userId);
        wishlistRepository.Add(wishlist);

        await unitOfWork.SaveChangesAsync(ct);
    }
}
