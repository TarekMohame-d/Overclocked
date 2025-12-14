using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.UserAggregate.Events;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate.ValueObjects;
using CartEntity = Overclocked.Domain.CartAggregate.Cart;
using WishlistEntity = Overclocked.Domain.WishlistAggregate.Wishlist;

namespace Overclocked.Application.Authentication.Commands.EventHandlers;

public class UserEmailConfirmedDomainEventHandler(
    ICartRepository cartRepository,
    IWishlistRepository wishlistRepository) : IDomainEventHandler<UserEmailConfirmedEvent>
{
    public async Task Handle(UserEmailConfirmedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var userId = UserId.Create(domainEvent.UserId);

        var existingCart = await cartRepository.ExistsAsync(userId, cancellationToken);
        if(!existingCart)
        {
            var cart = CartEntity.Create(CartId.Create(), userId);
            await cartRepository.AddAsync(cart, cancellationToken);
        }

        var existingWishlist = await wishlistRepository.ExistsAsync(userId, cancellationToken);
        if(!existingWishlist)
        {
            var wishlist = WishlistEntity.Create(WishlistId.Create(), userId);
            await wishlistRepository.AddAsync(wishlist, cancellationToken);
        }
    }
}
