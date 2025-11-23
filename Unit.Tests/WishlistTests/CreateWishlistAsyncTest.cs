// using Application.Abstraction.Repositories;
// using Application.Common.Results;
// using Application.Services.Wishlist;
// using Domain.Entities;
// using NSubstitute;
// using Shouldly;

// namespace Unit.Tests.WishlistTests;

// public class CreateWishlistAsyncTest
// {
//     private readonly IWishlistRepository _wishlistRepositoryMock;
//     private readonly IGenericRepository<WishlistItem> _wishlistItemRepositoryMock;
//     private readonly IProductRepository _productRepositoryMock;
//     private readonly IUnitOfWork _unitOfWorkMock;
//     private readonly WishlistService _wishlistService;

//     public CreateWishlistAsyncTest()
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
//     public async Task CreateWishlistAsync_Should_CreateWishlist_And_ReturnSuccess()
//     {
//         // Arrange
//         var userId = Guid.NewGuid();

//         // Act
//         var result = await _wishlistService.CreateWishlistAsync(userId, CancellationToken.None);

//         // Assert
//         result.IsSuccess.ShouldBeTrue();

//         await _wishlistRepositoryMock
//             .Received(1)
//             .AddAsync(
//                 Arg.Is<Wishlist>(w => w.UserId == userId),
//                 Arg.Any<CancellationToken>());
//     }
// }
