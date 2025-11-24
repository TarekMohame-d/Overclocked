using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Cart;
using Domain.Entities;
using Domain.Exceptions;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CartTests;

public class DeleteCartItemAsyncTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IGenericRepository<CartItem> _cartItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public DeleteCartItemAsyncTest()
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
    public async Task DeleteCartItemAsync_Should_ThrowException_When_ThereIsNoCart()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                asNoTracking: Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns((Cart)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _cartService.DeleteCartItemAsync(userId, productId, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<CartNotFoundException>();

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                asNoTracking: Arg.Any<bool>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCartItemAsync_Should_RemoveAndReturnSuccess_When_CartExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { UserId = userId };

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                asNoTracking: Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(cart);

        _cartItemRepositoryMock.DeleteWhereAsync(
                Arg.Any<Expression<Func<CartItem, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _cartService.DeleteCartItemAsync(userId, productId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                asNoTracking: Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _cartItemRepositoryMock.Received(1)
            .DeleteWhereAsync(
                Arg.Any<Expression<Func<CartItem, bool>>>(),
                Arg.Any<CancellationToken>());
    }
}
