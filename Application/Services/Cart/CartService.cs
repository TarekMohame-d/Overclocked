using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;

namespace Application.Services.Cart;

public sealed partial class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : ICartService;
