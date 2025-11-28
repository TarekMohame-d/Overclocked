using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Wishlist;
using Application.Services.Wishlist.DTOs.Request;
using Domain.Entities;
using Domain.Exceptions;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.WishlistTests;

public class DeleteWishlistItemAsyncTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IGenericRepository<WishlistItem> _wishlistItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly WishlistService _wishlistService;

    public DeleteWishlistItemAsyncTest()
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
    public async Task DeleteWishlistItemAsync_Should_ThrowException_When_WishlistNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wishlistItemId = Guid.NewGuid();

        var request = new DeleteWishlistItemRequest
        {
            WishlistItemId = wishlistItemId,
            UserId = userId
        };

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns((Wishlist)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _wishlistService.DeleteWishlistItemAsync(request, CancellationToken.None));

        // Assert
        exception.ShouldNotBeNull();
        exception.ShouldBeOfType<WishlistNotFoundException>();
        exception.Message.ShouldContain(userId.ToString());
    }

    [Fact]
    public async Task DeleteWishlistItemAsync_Should_DeleteSpecificItem_When_WishlistExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wishlistItemId = Guid.NewGuid();

        var request = new DeleteWishlistItemRequest
        {
            WishlistItemId = wishlistItemId,
            UserId = userId
        };

        var wishlist = new Wishlist { UserId = userId };

        _wishlistRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _wishlistItemRepositoryMock.DeleteWhereAsync(
                Arg.Any<Expression<Func<WishlistItem, bool>>>(),
                Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _wishlistService.DeleteWishlistItemAsync(request, CancellationToken.None);

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
