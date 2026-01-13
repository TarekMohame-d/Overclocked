using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Application.Features.WishlistUseCases.GetWishlistItems;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.WishlistTests;

public class GetWishlistItemsRequestHandlerTest
{
    private readonly IWishlistReadRepository _wishlistReadRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly GetWishlistItemsRequestHandler _getWishlistItemsRequestHandler;

    public GetWishlistItemsRequestHandlerTest()
    {
        _wishlistReadRepositoryMock = Substitute.For<IWishlistReadRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();

        _getWishlistItemsRequestHandler = new GetWishlistItemsRequestHandler(
            _wishlistReadRepositoryMock,
            _productReadRepositoryMock
        );
    }

    [Fact]
    public async Task GetWishlistItemsRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var wishlist = Wishlist.Create(userId);
        wishlist.AddWishlistItem(products[0].Id);
        wishlist.AddWishlistItem(products[1].Id);
        wishlist.AddWishlistItem(products[2].Id);

        var request = new GetWishlistItemsRequest { UserId = userId.Value };

        _wishlistReadRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(wishlist);

        _productReadRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns(products);

        // Act
        Result<IEnumerable<WishlistItemResponse>> result = await _getWishlistItemsRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count().ShouldBe(3);
        result.Error.ShouldBe(Error.None);

        await _wishlistReadRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productReadRepositoryMock.Received(1).GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());
    }
}
