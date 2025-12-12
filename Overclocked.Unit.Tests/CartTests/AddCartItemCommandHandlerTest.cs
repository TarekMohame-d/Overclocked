using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Cart.Commands;
using Overclocked.Application.Cart.Commands.AddCartItem;
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

public class AddCartItemCommandHandlerTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly ICartCommands _cartCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public AddCartItemCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();

        _cartCommands = new CartCommands(_cartRepositoryMock, _productRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task AddCartItemCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());
        var command = new AddCartItemCommand
        {
            UserId = userId.Value,
            ProductId = products[0].Id.Value,
            Quantity = 1
        };

        var cart = Cart.Create(CartId.Create(), userId);

        _cartRepositoryMock.GetCartAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(cart);

        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result<CartItemResponse> result = await _cartCommands
            .AddCartItemCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBe(Error.None);

        await _cartRepositoryMock.Received(1)
            .GetCartAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
