using Application.Common.Results;
using Application.Services.Cart.DTOs.Request;
using Application.Services.Cart.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface ICartService
{
    Task<Result> CreateCartAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result<CartItemResponse>> GetCartItemsAsync(Guid userId, CancellationToken cancellationToken);
    Task<Result> AddCartItemAsync(AddCartItemRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateCartItemAsync(UpdateCartItemRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteCartItemAsync(DeleteCartItemRequest request, CancellationToken cancellationToken);
    Task<Result> ClearCartAsync(Guid userId, CancellationToken cancellationToken);
}
