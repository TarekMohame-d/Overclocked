using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Wishlist.Commands.AddWishlistItem;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Wishlist;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.Domain.WishlistAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.WishlistTests;

public class AddWishlistItemCommandHandlerTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly AddWishlistItemCommandHandler _addWishlistItemCommandHandler;

    public AddWishlistItemCommandHandlerTest()
    {
        _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _addWishlistItemCommandHandler = new AddWishlistItemCommandHandler(
            _wishlistRepositoryMock,
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task AddWishlistItemCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());
        var command = new AddWishlistItemCommand
        {
            UserId = userId.Value,
            ProductId = products[0].Id.Value
        };

        var wishlist = Wishlist.Create(userId);

        _wishlistRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result<WishlistResponse> result = await _addWishlistItemCommandHandler
            .Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _wishlistRepositoryMock.Received(1)
            .GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
