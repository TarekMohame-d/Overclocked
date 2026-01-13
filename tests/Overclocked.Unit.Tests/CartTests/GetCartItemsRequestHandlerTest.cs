using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Application.Features.CartUseCases.GetCartItems;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CartTests;

public class GetCartItemsRequestHandlerTest
{
    private readonly ICartReadRepository _cartReadRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly GetCartItemsRequestHandler _getCartItemRequestHandler;

    public GetCartItemsRequestHandlerTest()
    {
        _cartReadRepositoryMock = Substitute.For<ICartReadRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();

        _getCartItemRequestHandler = new GetCartItemsRequestHandler(_cartReadRepositoryMock, _productReadRepositoryMock);
    }

    [Fact]
    public async Task GetCartItemRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var cart = Cart.Create(userId);
        Result<CartItemId> cartItemIdResult = cart.AddCartItem(products[0].Id, 1);
        cart.AddCartItem(products[1].Id, 4);
        cart.AddCartItem(products[2].Id, 2);

        var request = new GetCartItemsRequest { UserId = userId.Value };

        _cartReadRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);

        _productReadRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns(products);

        // Act
        Result<CartResponse> result = await _getCartItemRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.CartItems.Count().ShouldBe(3);
        result.Error.ShouldBe(Error.None);

        await _cartReadRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productReadRepositoryMock.Received(1).GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());
    }
}
