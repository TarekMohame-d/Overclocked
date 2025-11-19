using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Cart;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CartTests;

public class CreateCartAsyncTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public CreateCartAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _cartService = new CartService(_cartRepositoryMock, _productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateCartAsync_When_ThereIsNoError_Should_ReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cart = new Cart { UserId = userId };

        _cartRepositoryMock.AddAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>()).Returns(cart);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _cartService.CreateCartAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _cartRepositoryMock.Received(1).AddAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }
}
