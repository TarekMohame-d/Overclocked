using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Cart;
using Application.Services.Cart.DTOs.Response;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.Exceptions;
using MockQueryable;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.CartTests;

public class GetCartItemsAsyncTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IGenericRepository<CartItem> _cartItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CartService _cartService;

    public GetCartItemsAsyncTest()
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
    public async Task GetCartItemsAsync_Should_ThrowException_When_CartNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Cart)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _cartService.GetCartItemsAsync(userId, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<CartNotFoundException>();

        await _cartRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCartItemsAsync_Should_ReturnItems_When_CartExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var cart = new Cart { UserId = userId };

        _cartRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Cart, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(cart);

        List<Product> product = new ProductFaker().Generate(3);
        product[0].Price = 100;
        product[0].Discount = 0.1m;
        product[1].Price = 200;
        product[1].Discount = 0.2m;
        product[2].Price = 300;
        product[2].Discount = 0.3m;

        var cartItems = new List<CartItem>
        {
            new() {
                CartId = cart.Id,
                ProductId = product[0].Id,
                Quantity = 2,
                Product = product[0]
            },
            new() {
                CartId = cart.Id,
                ProductId = product[1].Id,
                Quantity = 1,
                Product = product[1]
            },
            new() {
                CartId = cart.Id,
                ProductId = product[2].Id,
                Quantity = 3,
                Product = product[2]
            }
        };

        IQueryable<CartItem> mockQueryable = cartItems.BuildMock();

        _cartItemRepositoryMock.Query()
            .Returns(mockQueryable);

        // Act
        Result<CartItemResponse> result = await _cartService.GetCartItemsAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Data.CartItems.Count().ShouldBe(3);
        CartItemResponse.CartItem item = result.Data.CartItems.First();
        item.LineTotal.ShouldBe(180); // 100 * (1 - 0.1) * 2 = 100 * 0.9 * 2 = 180
        result.Data.Total.ShouldBe(CalculateSubtotal(cartItems));
    }

    private static decimal CalculateSubtotal(List<CartItem> cartItems)
    {
        var subTotal = 0m;
        foreach(CartItem cartItem in cartItems)
        {
            subTotal += cartItem.Product!.Price * (1m - cartItem.Product.Discount) * cartItem.Quantity;
        }

        return subTotal;
    }
}
