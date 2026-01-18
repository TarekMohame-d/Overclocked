using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;

namespace Overclocked.Application.Features.WishlistUseCases.ClearWishlist;

public class ClearWishlistRequestHandler(IWishlistRepository wishlistRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ClearWishlistRequest>
{
    public async Task<Result> Handle(ClearWishlistRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);

        Wishlist wishlist = await wishlistRepository.GetAsync(userId, ct) ?? throw new WishlistNotFoundException(request.UserId);

        wishlist.ClearWishlist();

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
