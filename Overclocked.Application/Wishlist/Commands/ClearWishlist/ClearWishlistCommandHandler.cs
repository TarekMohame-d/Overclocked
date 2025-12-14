using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.ValueObjects;
using WishlistEntity = Overclocked.Domain.WishlistAggregate.Wishlist;

namespace Overclocked.Application.Wishlist.Commands.ClearWishlist;

public class ClearWishlistCommandHandler(
    IWishlistRepository wishlistRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ClearWishlistCommand>
{
    public async Task<Result> Handle(ClearWishlistCommand command, CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);

        WishlistEntity wishlist = await wishlistRepository.GetAsync(userId, cancellationToken)
            ?? throw new WishlistNotFoundException(command.UserId);

        wishlist.ClearWishlist();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
