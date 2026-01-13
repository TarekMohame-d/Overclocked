using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Application.Features.CartUseCases.UpdateCartItem;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CartTests;

public class UpdateCartItemRequestHandlerTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateCartItemRequestHandler _updateCartItemRequestHandler;

    public UpdateCartItemRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();

        _updateCartItemRequestHandler = new UpdateCartItemRequestHandler(
            _cartRepositoryMock,
            _unitOfWorkMock,
            _productReadRepositoryMock
        );
    }

    [Fact]
    public async Task UpdateCartItemRequestHandler_Should_ReturnFailure_When_ValidationFail()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var cart = Cart.Create(userId);
        Result<CartItemId> cartItemIdResult = cart.AddCartItem(products[0].Id, 1);

        var request = new UpdateCartItemRequest
        {
            UserId = userId.Value,
            CartItemId = cartItemIdResult.Value.Value,
            Quantity = 0,
        };

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        Result<CartResponse> result = await _updateCartItemRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        cart.CartItems.First(ci => ci.Id == cartItemIdResult.Value).Quantity.ShouldBe(1);
        result.Error.ShouldNotBe(Error.None);

        await _cartRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCartItemRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var cart = Cart.Create(userId);
        Result<CartItemId> cartItemIdResult = cart.AddCartItem(products[0].Id, 1);

        var request = new UpdateCartItemRequest
        {
            UserId = userId.Value,
            CartItemId = cartItemIdResult.Value.Value,
            Quantity = 2,
        };

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);

        _productReadRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result<CartResponse> result = await _updateCartItemRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        cart.CartItems.First(ci => ci.Id == cartItemIdResult.Value).Quantity.ShouldBe(2);
        result.Error.ShouldBe(Error.None);

        await _cartRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productReadRepositoryMock.Received(1).GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
