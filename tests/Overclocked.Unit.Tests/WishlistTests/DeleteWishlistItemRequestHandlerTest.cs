using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.WishlistUseCases.DeleteWishlistItem;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Domain.WishlistAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.WishlistTests;

public class DeleteWishlistItemRequestHandlerTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IProductReadRepository _productReadRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteWishlistItemRequestHandler _deleteWishlistItemRequestHandler;

    public DeleteWishlistItemRequestHandlerTest()
    {
        _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();
        _productReadRepositoryMock = Substitute.For<IProductReadRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteWishlistItemRequestHandler = new DeleteWishlistItemRequestHandler(
            _wishlistRepositoryMock,
            _productReadRepositoryMock,
            _unitOfWorkMock
        );
    }

    [Fact]
    public async Task DeleteWishlistItemRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var wishlist = Wishlist.Create(userId);
        wishlist.AddWishlistItem(products[0].Id);
        wishlist.AddWishlistItem(products[1].Id);
        wishlist.AddWishlistItem(products[2].Id);

        var request = new DeleteWishlistItemRequest { UserId = userId.Value, ProductId = products[0].Id.Value };

        _wishlistRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(wishlist);

        _productReadRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result<IEnumerable<WishlistItemResponse>> result = await _deleteWishlistItemRequestHandler.Handle(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count().ShouldBe(2);
        result.Error.ShouldBe(Error.None);

        await _wishlistRepositoryMock.Received(1).GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productReadRepositoryMock.Received(1).GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
