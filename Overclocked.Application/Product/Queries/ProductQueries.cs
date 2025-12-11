using Overclocked.Application.Abstraction.Persistence;

namespace Overclocked.Application.Product.Queries;

public sealed partial class ProductQueries(
    IProductRepository productRepository,
    IReviewRepository reviewRepository) : IProductQueries;
