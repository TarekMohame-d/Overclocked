using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Wishlist;
using Domain.Entities;
using Domain.Exceptions;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.WishlistTests;

public class ClearWishlistAsyncTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IGenericRepository<WishlistItem> _wishlistItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly WishlistService _wishlistService;

    public ClearWishlistAsyncTest()
    {
        _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();
        _wishlistItemRepositoryMock = Substitute.For<IGenericRepository<WishlistItem>>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _wishlistService = new WishlistService(
            _wishlistRepositoryMock,
            _wishlistItemRepositoryMock,
            _productRepositoryMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task ClearWishlistAsync_Should_ThrowException_When_WishlistNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Wishlist)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _wishlistService.ClearWishlistAsync(userId, CancellationToken.None));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<WishlistNotFoundException>();
        exception.Message.ShouldContain(userId.ToString());

        await _wishlistRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ClearWishlistAsync_Should_ClearItems_When_WishlistExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wishlist = new Wishlist { UserId = userId };

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _wishlistItemRepositoryMock.DeleteWhereAsync(
                Arg.Any<Expression<Func<WishlistItem, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(10);

        // Act
        Result result = await _wishlistService.ClearWishlistAsync(userId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _wishlistRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());

        await _wishlistItemRepositoryMock.Received(1)
            .DeleteWhereAsync(
                Arg.Any<Expression<Func<WishlistItem, bool>>>(),
                Arg.Any<CancellationToken>());
    }
}
