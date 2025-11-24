using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Wishlist;
using Application.Services.Wishlist.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.Exceptions;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.WishlistTests;

public class AddWishlistItemAsyncTest
{
    private readonly IWishlistRepository _wishlistRepositoryMock;
    private readonly IGenericRepository<WishlistItem> _wishlistItemRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly WishlistService _wishlistService;

    public AddWishlistItemAsyncTest()
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
    public async Task AddWishlistItemAsync_Should_ThrowException_When_ThereIsNoWishlist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addWishlistItemRequest = new AddWishlistItemRequest { ProductId = productId };

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Wishlist?>(null));

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _wishlistService.AddWishlistItemAsync(userId, addWishlistItemRequest, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<WishlistNotFoundException>();

        await _wishlistRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddWishlistItemAsync_Should_ReturnError_When_ProductNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addWishlistItemRequest = new AddWishlistItemRequest { ProductId = productId };

        var wishlist = new Wishlist { UserId = userId };

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _productRepositoryMock.AnyAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _wishlistService
            .AddWishlistItemAsync(userId, addWishlistItemRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        await _wishlistRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .AnyAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddWishlistItemAsync_Should_AddWishlistItem_When_WishlistItemNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addWishlistItemRequest = new AddWishlistItemRequest { ProductId = productId };

        var wishlist = new Wishlist { UserId = userId };

        Product product = new ProductFaker().Generate();

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _productRepositoryMock.AnyAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        _wishlistItemRepositoryMock.AnyAsync(
                Arg.Any<Expression<Func<WishlistItem, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(false);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _wishlistService
            .AddWishlistItemAsync(userId, addWishlistItemRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), cancellationToken: Arg.Any<CancellationToken>());

        await _wishlistItemRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<WishlistItem, bool>>>(), cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddWishlistItemAsync_Should_ReturnSuccess_When_WishlistItemAlreadyExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var addWishlistItemRequest = new AddWishlistItemRequest { ProductId = productId };

        var wishlist = new Wishlist { UserId = userId };

        Product product = new ProductFaker().Generate();

        _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(wishlist);

        _productRepositoryMock.AnyAsync(
                Arg.Any<Expression<Func<Product, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        _wishlistItemRepositoryMock.AnyAsync(
                Arg.Any<Expression<Func<WishlistItem, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _wishlistService
            .AddWishlistItemAsync(userId, addWishlistItemRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _wishlistRepositoryMock.SingleOrDefaultAsync(
                Arg.Any<Expression<Func<Wishlist, bool>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>());

        await _productRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Product, bool>>>(), cancellationToken: Arg.Any<CancellationToken>());

        await _wishlistItemRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<WishlistItem, bool>>>(), cancellationToken: Arg.Any<CancellationToken>());

        await _unitOfWorkMock.DidNotReceive()
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
