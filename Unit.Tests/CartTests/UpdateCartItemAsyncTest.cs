using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Cart;
using Application.Services.Cart.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.Exceptions;
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

        var updateCartRequest = new UpdateCartItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 };

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns((Cart)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _cartService.UpdateCartItemAsync(userId, updateCartRequest, CancellationToken.None));

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

        var updateCartRequest = new UpdateCartItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 };

        var cart = new Cart { UserId = userId };

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Product)null!);

        // Act
        Result result = await _cartService.UpdateCartItemAsync(userId, updateCartRequest, CancellationToken.None);

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
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCartItemAsync_Should_ReturnSuccess_When_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var updateCartRequest = new UpdateCartItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 };

        var cart = new Cart { UserId = userId };

        Product product = new ProductFaker().Generate();

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(product);

        _unitOfWorkMock.CompleteAsync(cancellationToken: Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _cartService.UpdateCartItemAsync(userId, updateCartRequest, CancellationToken.None);

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
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCartItemAsync_Should_ReturnError_When_QuantityIsGreaterThanStock()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var updateCartRequest = new UpdateCartItemRequest { ProductId = productId, Quantity = 200 };

        var cart = new Cart { UserId = userId };

        Product product = new ProductFaker().Generate();

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                Arg.Any<Func<IQueryable<Cart>, IQueryable<Cart>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(product);

        // Act
        Result result = await _cartService.UpdateCartItemAsync(userId, updateCartRequest, CancellationToken.None);

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
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.DidNotReceive()
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
