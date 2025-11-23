// using System.Linq.Expressions;
// using Application.Abstraction.Repositories;
// using Application.Common.Results;
// using Application.Services.Wishlist;
// using Application.Services.Wishlist.DTOs.Response;
// using ArchitectureTests.FakeData;
// using Domain.Entities;
// using Domain.Exceptions;
// using MockQueryable;
// using NSubstitute;
// using Shouldly;

// namespace Unit.Tests.WishlistTests;

// public class GetWishlistItemsAsyncTest
// {
//     private readonly IWishlistRepository _wishlistRepositoryMock;
//     private readonly IGenericRepository<WishlistItem> _wishlistItemRepositoryMock;
//     private readonly IProductRepository _productRepositoryMock;
//     private readonly IUnitOfWork _unitOfWorkMock;
//     private readonly WishlistService _wishlistService;

//     public GetWishlistItemsAsyncTest()
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
//     public async Task GetWishlistItemsAsync_When_WishlistNotFound_Should_ThrowException()
//     {
//         // Arrange
//         var userId = Guid.NewGuid();

//         _wishlistRepositoryMock
//             .SingleOrDefaultAsync(
//                 Arg.Any<Expression<Func<Wishlist, bool>>>(),
//                 cancellationToken: Arg.Any<CancellationToken>())
//             .Returns((Wishlist)null!);

//         // Act
//         var exception = await Should.ThrowAsync<WishlistNotFoundException>(async () =>
//             await _wishlistService.GetWishlistItemsAsync(userId, CancellationToken.None));

//         // Assert
//         exception.ShouldNotBeNull();
//         exception.Message.ShouldContain(userId.ToString());
//     }

//     [Fact]
//     public async Task GetWishlistItemsAsync_When_WishlistExists_Should_ReturnItems()
//     {
//         // Arrange
//         var userId = Guid.NewGuid();
//         var wishlist = new Wishlist { UserId = userId };
//         var product = new ProductFaker().Generate();
//         var wishlistItems = new List<WishlistItem>
//         {
//             new WishlistItem { WishlistId = wishlist.Id, ProductId = product.Id, Product = product }
//         };

//         _wishlistRepositoryMock
//             .SingleOrDefaultAsync(
//                 Arg.Any<Expression<Func<Wishlist, bool>>>(),
//                 cancellationToken: Arg.Any<CancellationToken>())
//             .Returns(wishlist);

//         var mockQueryable = wishlistItems.BuildMock();
//         _wishlistItemRepositoryMock.Query().Returns(mockQueryable);

//         // Act
//         var result = await _wishlistService.GetWishlistItemsAsync(userId, CancellationToken.None);

//         // Assert
//         result.IsSuccess.ShouldBeTrue();
//         result.Data.ShouldNotBeNull();
//         result.Data.Count().ShouldBe(1);
//         result.Data.First().ProductId.ShouldBe(product.Id);
//     }
// }
