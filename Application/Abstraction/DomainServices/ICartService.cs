using Application.Common.Results;
using Application.Services.Cart.DTOs.Request;
using Application.Services.Cart.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface ICartService
{
    Task<Result> CreateCartAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<IEnumerable<CartItemResponse>>> GetCartItemsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> AddCartItemAsync(Guid userId, AddCartItemRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateCartItemAsync(Guid userId, UpdateCartItemRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteCartItemAsync(Guid userId, Guid productId, CancellationToken cancellationToken);
    Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken);
}
