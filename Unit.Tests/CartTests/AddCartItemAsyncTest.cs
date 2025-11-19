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

public class AddCartItemAsyncTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public AddCartItemAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _cartService = new CartService(_cartRepositoryMock, _productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task AddCartItemAsync_When_ThereIsNoCart_Should_ThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addCartItemRequest = new AddCartItemRequest { ProductId = productId, Quantity = 1 };

        _cartRepositoryMock
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Cart)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _cartService.AddCartItemAsync(userId, addCartItemRequest, CancellationToken.None)
        );

        // Assert
        exception.ShouldBeOfType<CartNotFoundException>();

        await _cartRepositoryMock
            .Received(1)
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCartItemAsync_When_ProductNotFound_Should_ReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addCartItemRequest = new AddCartItemRequest { ProductId = productId, Quantity = 1 };

        var cart = new Cart { UserId = userId };

        _cartRepositoryMock
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns((Product)null!);

        // Act
        Result result = await _cartService.AddCartItemAsync(userId, addCartItemRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await _cartRepositoryMock
            .Received(1)
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());

        await _productRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task AddCartItemAsync_When_CartItemNotFound_Should_AddCartItem()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addCartItemRequest = new AddCartItemRequest { ProductId = productId, Quantity = 1 };

        var cart = new Cart { UserId = userId };

        Product product = new ProductFaker().Generate();

        _cartRepositoryMock
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(product);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _cartService.AddCartItemAsync(userId, addCartItemRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _cartRepositoryMock
            .Received(1)
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());

        await _productRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCartItemAsync_When_QuantityIsGreaterThanStock_Should_ReturnError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addCartItemRequest = new AddCartItemRequest { ProductId = productId, Quantity = 200 };

        var cart = new Cart { UserId = userId };

        Product product = new ProductFaker().Generate();

        _cartRepositoryMock
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(product);

        // Act
        Result result = await _cartService.AddCartItemAsync(userId, addCartItemRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await _cartRepositoryMock
            .Received(1)
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());

        await _productRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _unitOfWorkMock.DidNotReceive().CompleteAsync(Arg.Any<CancellationToken>());
    }
}
