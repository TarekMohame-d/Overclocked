using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Contracts.Wishlist;
using Overclocked.Domain.Common.Exceptions;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using WishlistEntity = Overclocked.Domain.WishlistAggregate.Wishlist;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;
using Overclocked.Application.Wishlist.Mapping;

namespace Overclocked.Application.Wishlist.Commands.DeleteWishlistItem;

public class DeleteWishlistItemCommandHandler(
    IWishlistRepository wishlistRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteWishlistItemCommand, WishlistResponse>
{
    public async Task<Result<WishlistResponse>> Handle(
        DeleteWishlistItemCommand command,
        CancellationToken cancellationToken)
    {
        var userId = UserId.Create(command.UserId);
        var productId = ProductId.Create(command.ProductId);

        WishlistEntity wishlist = await wishlistRepository.GetAsync(userId, cancellationToken)
            ?? throw new WishlistNotFoundException(command.UserId);

        wishlist.RemoveWishlistItem(productId);

        var productIds = wishlist.WishlistItems.Select(x => x.ProductId).ToList();
        List<ProductEntity> products = await productRepository.GetByIdsAsync(productIds, cancellationToken);

        WishlistResponse response = WishlistMapper.MapToResponse(wishlist, products);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(response);
    }
}
