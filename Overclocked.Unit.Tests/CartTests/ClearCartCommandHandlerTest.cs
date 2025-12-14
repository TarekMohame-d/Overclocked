using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Cart.Commands.ClearCart;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.CartTests;

public class ClearCartCommandHandlerTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ClearCartCommandHandler _clearCartItemCommandHandler;

    public ClearCartCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();

        _clearCartItemCommandHandler = new ClearCartCommandHandler(
            _cartRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task ClearCartCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var cart = Cart.Create(CartId.Create(), userId);
        cart.AddCartItem(products[0].Id, 1);
        cart.AddCartItem(products[1].Id, 4);
        cart.AddCartItem(products[2].Id, 2);

        var command = new ClearCartCommand
        {
            UserId = userId.Value
        };

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(cart);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _clearCartItemCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        cart.CartItems.ShouldBeEmpty();
        result.Error.ShouldBe(Error.None);

        await _cartRepositoryMock.Received(1)
            .GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
