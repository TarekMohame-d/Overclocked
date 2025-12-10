using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Product.Commands;

public sealed partial class ProductCommands(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork) : IProductCommands;
