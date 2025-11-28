using Application.Common.Results;
using Application.Services.Wishlist.DTOs.Request;
using Application.Services.Wishlist.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface IWishlistService
{
    Task<Result> CreateWishlistAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<WishlistItemResponse>>> GetWishlistItemsAsync(
        Guid userId,
        CancellationToken cancellationToken);
    Task<Result> AddWishlistItemAsync(AddWishlistItemRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteWishlistItemAsync(DeleteWishlistItemRequest request, CancellationToken cancellationToken);
    Task<Result> ClearWishlistAsync(Guid userId, CancellationToken cancellationToken);
}
