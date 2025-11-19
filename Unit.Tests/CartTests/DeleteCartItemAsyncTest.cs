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
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public DeleteCartItemAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _cartService = new CartService(_cartRepositoryMock, _productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteCartItemAsync_When_ThereIsNoCart_Should_ThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _cartRepositoryMock
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Cart)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _cartService.DeleteCartItemAsync(userId, productId, CancellationToken.None)
        );

        // Assert
        exception.ShouldBeOfType<CartNotFoundException>();

        await _cartRepositoryMock
            .Received(1)
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCartItemAsync_When_CartExists_Should_RemoveAndReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var cart = new Cart { UserId = userId };

        _cartRepositoryMock
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cart);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _cartService.DeleteCartItemAsync(userId, productId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _cartRepositoryMock
            .Received(1)
            .GetCartWithItemsAsync(Arg.Any<Guid>(), cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
