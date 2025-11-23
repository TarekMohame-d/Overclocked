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
    private readonly IGenericRepository<CartItem> _cartItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public CreateCartAsyncTest()
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
    public async Task CreateCartAsync_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cart = new Cart { UserId = userId };

        _cartRepositoryMock.AddAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
            .Returns(cart);

        // Act
        Result result = await _cartService.CreateCartAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _cartRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }
}
