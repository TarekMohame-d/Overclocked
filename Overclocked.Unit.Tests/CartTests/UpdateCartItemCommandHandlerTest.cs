using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Cart.Commands.UpdateCartItem;
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

public class UpdateCartItemCommandHandlerTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly UpdateCartItemCommandHandler _updateCartItemCommandHandler;

    public UpdateCartItemCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();

        _updateCartItemCommandHandler = new UpdateCartItemCommandHandler(
            _cartRepositoryMock,
            _unitOfWorkMock,
            _productRepositoryMock);
    }

    [Fact]
    public async Task UpdateCartItemCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var cart = Cart.Create(CartId.Create(), userId);
        CartItemId cartItemId = cart.AddCartItem(products[0].Id, 1);

        var command = new UpdateCartItemCommand
        {
            UserId = userId.Value,
            CartItemId = cartItemId,
            Quantity = 2
        };

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result<CartResponse> result = await _updateCartItemCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.CartItems.First(ci => ci.CartItemId == cartItemId).Quantity.ShouldBe(2);
        result.Error.ShouldBe(Error.None);

        await _cartRepositoryMock.Received(1)
            .GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
