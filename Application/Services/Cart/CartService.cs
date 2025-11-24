using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;
using Domain.Entities;

namespace Application.Services.Cart;

public sealed partial class CartService(
    ICartRepository cartRepository,
    IGenericRepository<CartItem> cartItemRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
    : ICartService;
