using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Cart.Commands;

public sealed partial class CartCommands(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : ICartCommands;
