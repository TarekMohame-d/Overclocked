using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Wishlist.Queries.GetWishlistItems;
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

public class GetWishlistItemsQueryHandlerTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly GetWishlistItemsQueryHandler _getWishlistItemsQueryHandler;

    public GetWishlistItemsQueryHandlerTest()
    {
        _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();

        _getWishlistItemsQueryHandler = new GetWishlistItemsQueryHandler(
            _wishlistRepositoryMock,
            _productRepositoryMock);
    }

    [Fact]
    public async Task GetWishlistItemsQueryHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var wishlist = Wishlist.Create(userId);
        wishlist.AddWishlistItem(products[0].Id);
        wishlist.AddWishlistItem(products[1].Id);
        wishlist.AddWishlistItem(products[2].Id);

        var command = new GetWishlistItemsQuery
        {
            UserId = userId.Value
        };

        _wishlistRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        // Act
        Result<WishlistResponse> result = await _getWishlistItemsQueryHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.WishlistItems.Count().ShouldBe(3);
        result.Error.ShouldBe(Error.None);

        await _wishlistRepositoryMock.Received(1)
            .GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());
    }
}
