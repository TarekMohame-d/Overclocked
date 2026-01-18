using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.WishlistUseCases.ClearWishlist;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.WishlistTests;

public class ClearWishlistRequestHandlerTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ClearWishlistRequestHandler _clearWishlistRequestHandler;

    public ClearWishlistRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();

        _clearWishlistRequestHandler = new ClearWishlistRequestHandler(_wishlistRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task ClearWishlistRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var wishlist = Wishlist.Create(userId);
        wishlist.AddWishlistItem(products[0].Id);
        wishlist.AddWishlistItem(products[1].Id);
        wishlist.AddWishlistItem(products[2].Id);

        var request = new ClearWishlistRequest { UserId = userId.Value };

        _wishlistRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(wishlist);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _clearWishlistRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        wishlist.WishlistItems.ShouldBeEmpty();
        result.Error.ShouldBe(Error.None);

        await _wishlistRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
