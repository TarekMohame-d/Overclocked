// using System.Linq.Expressions;
// using Application.Abstraction.Repositories;
// using Application.Common.Results;
// using Application.Services.Wishlist;
// using Domain.Entities;
// using Domain.Exceptions;
// using NSubstitute;
// using Shouldly;

// namespace Unit.Tests.WishlistTests;

// public class DeleteWishlistItemAsyncTest
// {
//     private readonly IWishlistRepository _wishlistRepositoryMock;
//     private readonly IGenericRepository<WishlistItem> _wishlistItemRepositoryMock;
//     private readonly IProductRepository _productRepositoryMock;
//     private readonly IUnitOfWork _unitOfWorkMock;
//     private readonly WishlistService _wishlistService;

//     public DeleteWishlistItemAsyncTest()
//     {
//         _wishlistRepositoryMock = Substitute.For<IWishlistRepository>();
//         _wishlistItemRepositoryMock = Substitute.For<IGenericRepository<WishlistItem>>();
//         _productRepositoryMock = Substitute.For<IProductRepository>();
//         _unitOfWorkMock = Substitute.For<IUnitOfWork>();

//         _wishlistService = new WishlistService(
//             _wishlistRepositoryMock,
//             _wishlistItemRepositoryMock,
//             _productRepositoryMock,
//             _unitOfWorkMock);
//     }

//     [Fact]
//     public async Task DeleteWishlistItemAsync_When_WishlistNotFound_Should_ThrowException()
//     {
//         // Arrange
//         var userId = Guid.NewGuid();
//         var productId = Guid.NewGuid();

//         _wishlistRepositoryMock
//             .SingleOrDefaultAsync(
//                 Arg.Any<Expression<Func<Wishlist, bool>>>(),
//                 cancellationToken: Arg.Any<CancellationToken>())
//             .Returns((Wishlist)null!);

//         // Act
//         var exception = await Should.ThrowAsync<WishlistNotFoundException>(async () =>
//             await _wishlistService.DeleteWishlistItemAsync(userId, productId, CancellationToken.None));

//         // Assert
//         exception.ShouldNotBeNull();
//         exception.Message.ShouldContain(userId.ToString());
//     }

//     [Fact]
//     public async Task DeleteWishlistItemAsync_When_WishlistExists_Should_DeleteSpecificItem_And_ReturnSuccess()
//     {
//         // Arrange
//         var userId = Guid.NewGuid();
//         var productId = Guid.NewGuid();
//         var wishlist = new Wishlist { Id = Guid.NewGuid(), UserId = userId };

//         _wishlistRepositoryMock
//             .SingleOrDefaultAsync(
//                 Arg.Any<Expression<Func<Wishlist, bool>>>(),
//                 cancellationToken: Arg.Any<CancellationToken>())
//             .Returns(wishlist);

//         // Act
//         var result = await _wishlistService.DeleteWishlistItemAsync(userId, productId, CancellationToken.None);

//         // Assert
//         result.IsSuccess.ShouldBeTrue();

//         await _wishlistItemRepositoryMock
//             .Received(1)
//             .DeleteWhereAsync(
//                 Arg.Any<Expression<Func<WishlistItem, bool>>>(),
//                 Arg.Any<CancellationToken>());
//     }
// }
