using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Cart.DTOs.Request;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Cart;

public sealed partial class CartService
{
    public async Task<Result> AddCartItemAsync(
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Cart cart =
            await cartRepository.SingleOrDefaultAsync(
                x => x.UserId == request.UserId,
                include: q => q.Include(c => c.CartItems),
                asNoTracking: false,
                cancellationToken)
            ?? throw new CartNotFoundException(request.UserId);

        var stockQuantity = await productRepository.GetProductStockQuantityAsync(request.ProductId, cancellationToken);

        if(stockQuantity is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        try
        {
            cart.AddCartItem(request.ProductId, request.Quantity, (int)stockQuantity);
        }
        catch(InvalidCartItemQuantityException)
        {
            return Result.Failure(Errors.InvalidCartItemQuantity);
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
