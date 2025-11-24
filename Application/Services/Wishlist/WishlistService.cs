using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;
using Domain.Entities;

namespace Application.Services.Wishlist;

public sealed partial class WishlistService(
    IWishlistRepository wishlistRepository,
    IGenericRepository<WishlistItem> wishlistItemRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : IWishlistService;
