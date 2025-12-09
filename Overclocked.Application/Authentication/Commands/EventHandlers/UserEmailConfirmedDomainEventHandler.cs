using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;

namespace Overclocked.Application.Authentication.Commands.EventHandlers;

public class UserEmailConfirmedDomainEventHandler(
    ICartRepository cartRepository,
    IWishlistRepository wishlistRepository) : IDomainEventHandler<UserEmailConfirmedEvent>
{
    public async Task Handle(UserEmailConfirmedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(domainEvent.UserId);

        Cart? existingCart = await cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if(existingCart is null)
        {
            var cart = Cart.Create(CartId.Create(), userId);
            await cartRepository.AddAsync(cart, cancellationToken);
        }

        Wishlist? existingWishlist = await wishlistRepository.GetByUserIdAsync(userId, cancellationToken);
        if(existingWishlist is null)
        {
            var wishlist = Wishlist.Create(WishlistId.Create(), userId);
            await wishlistRepository.AddAsync(wishlist, cancellationToken);
        }
    }
}
