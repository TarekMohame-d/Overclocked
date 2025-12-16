using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Cart.Queries.GetCartItems;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.CartTests;

public class GetCartItemsQueryHandlerTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly GetCartItemsQueryHandler _getCartItemQueryHandler;

    public GetCartItemsQueryHandlerTest()
    {
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();

        _getCartItemQueryHandler = new GetCartItemsQueryHandler(
            _cartRepositoryMock,
            _productRepositoryMock);
    }

    [Fact]
    public async Task GetCartItemQueryHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var cart = Cart.Create(userId);
        CartItemId cartItemId = cart.AddCartItem(products[0].Id, 1);
        cart.AddCartItem(products[1].Id, 4);
        cart.AddCartItem(products[2].Id, 2);

        var command = new GetCartItemsQuery
        {
            UserId = userId.Value
        };

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        // Act
        Result<CartResponse> result = await _getCartItemQueryHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.CartItems.Count().ShouldBe(3);
        result.Error.ShouldBe(Error.None);

        await _cartRepositoryMock.Received(1)
            .GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());
    }
}
