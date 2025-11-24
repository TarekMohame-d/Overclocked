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
        Guid userId,
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Cart cart =
            await cartRepository.SingleOrDefaultAsync(
                x => x.UserId == userId,
                include: q => q.Include(c => c.CartItems),
                asNoTracking: false,
                cancellationToken)
            ?? throw new CartNotFoundException(userId);

        Domain.Entities.Product? product = await productRepository
            .SingleOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken: cancellationToken);

        if(product is null)
            return Result.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound);

        try
        {
            cart.AddOrUpdateItem(request.ProductId, request.Quantity, product.StockQuantity);
        }
        catch(InvalidCartItemQuantityException)
        {
            return Result.Failure(Errors.InvalidCartItemQuantity);
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
