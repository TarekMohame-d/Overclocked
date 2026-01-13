using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.CartUseCases.AddCartItem;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.CartTests;

public class AddCartItemRequestHandlerTest
{
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly AddCartItemRequestHandler _addCartItemRequestHandler;

    public AddCartItemRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();

        _addCartItemRequestHandler = new AddCartItemRequestHandler(
            _cartRepositoryMock,
            _unitOfWorkMock,
            _productReadRepositoryMock
        );
    }

    [Fact]
    public async Task AddCartItemRequestHandler_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var request = new AddCartItemRequest
        {
            UserId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 1,
        };
        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result<CartResponse> result = await _addCartItemRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCartItemRequestHandler_Should_ReturnFailure_When_AddCartItemFail()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());

        var request = new AddCartItemRequest
        {
            UserId = userId.Value,
            ProductId = Guid.NewGuid(),
            Quantity = 0,
        };
        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        var cart = Cart.Create(userId);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        Result<CartResponse> result = await _addCartItemRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _cartRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddCartItemRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());
        var request = new AddCartItemRequest
        {
            UserId = userId.Value,
            ProductId = products[0].Id.Value,
            Quantity = 1,
        };

        _productReadRepositoryMock.ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>()).Returns(true);

        var cart = Cart.Create(userId);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);

        _productReadRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result<CartResponse> result = await _addCartItemRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _productReadRepositoryMock.Received(1).ExistsAsync(Arg.Any<ProductId>(), Arg.Any<CancellationToken>());

        await _cartRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productReadRepositoryMock.Received(1).GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
