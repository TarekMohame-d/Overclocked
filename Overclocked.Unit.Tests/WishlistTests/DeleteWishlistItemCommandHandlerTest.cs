using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Wishlist.Commands.DeleteWishlistItem;
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

public class DeleteWishlistItemCommandHandlerTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteWishlistItemCommandHandler _deleteWishlistItemCommandHandler;

    public DeleteWishlistItemCommandHandlerTest()
    {
        _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _deleteWishlistItemCommandHandler = new DeleteWishlistItemCommandHandler(
            _wishlistRepositoryMock,
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteWishlistItemCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        List<Product> products = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate(3);
        var userId = UserId.Create(Guid.NewGuid());

        var wishlist = Wishlist.Create(WishlistId.Create(), userId);
        wishlist.AddWishlistItem(products[0].Id);
        wishlist.AddWishlistItem(products[1].Id);
        wishlist.AddWishlistItem(products[2].Id);

        var command = new DeleteWishlistItemCommand
        {
            UserId = userId.Value,
            ProductId = products[0].Id
        };

        _wishlistRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(products);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result<WishlistResponse> result = await _deleteWishlistItemCommandHandler
            .Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.WishlistItems.Count().ShouldBe(2);
        result.Error.ShouldBe(Error.None);

        await _wishlistRepositoryMock.Received(1)
            .GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
