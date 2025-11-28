using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Cart;
using Application.Services.Cart.DTOs.Request;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CartTests;

public class UpdateCartItemAsyncTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IGenericRepository<CartItem> _cartItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public UpdateCartItemAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _cartItemRepositoryMock = Substitute.For<IGenericRepository<CartItem>>();
        _productRepositoryMock = Substitute.For<IProductRepository>();

        _cartService = new CartService(
            _cartRepositoryMock,
            _cartItemRepositoryMock,
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateCartItemAsync_Should_ThrowException_When_ThereIsNoCart()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var request = new UpdateCartItemRequest
        {
            UserId = userId,
            CartItemId = Guid.NewGuid(),
            Quantity = 1
        };

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns((Cart)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _cartService.UpdateCartItemAsync(request, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<CartNotFoundException>();

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCartItemAsync_Should_ReturnError_When_ProductDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { UserId = userId };

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = productId,
            Quantity = 1
        };

        var request = new UpdateCartItemRequest
        {
            UserId = userId,
            CartItemId = cartItem.Id,
            Quantity = 1
        };


        cart.CartItems.Add(cartItem);

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.GetProductStockQuantityAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<int?>(null));

        // Act
        Result result = await _cartService.UpdateCartItemAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetProductStockQuantityAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCartItemAsync_Should_ReturnSuccess_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { UserId = userId };

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = productId,
            Quantity = 1
        };

        var request = new UpdateCartItemRequest
        {
            UserId = userId,
            CartItemId = cartItem.Id,
            Quantity = 1
        };

        cart.CartItems.Add(cartItem);

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.GetProductStockQuantityAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(50);

        _unitOfWorkMock.CompleteAsync(cancellationToken: Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _cartService.UpdateCartItemAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetProductStockQuantityAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCartItemAsync_Should_ReturnError_When_QuantityIsGreaterThanStock()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { UserId = userId };

        var cartItem = new CartItem
        {
            CartId = cart.Id,
            ProductId = productId,
            Quantity = 1
        };

        var request = new UpdateCartItemRequest
        {
            UserId = userId,
            CartItemId = cartItem.Id,
            Quantity = 200
        };

        cart.CartItems.Add(cartItem);

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.GetProductStockQuantityAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(50);

        // Act
        Result result = await _cartService.UpdateCartItemAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetProductStockQuantityAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.DidNotReceive()
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
