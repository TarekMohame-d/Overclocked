using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;

namespace Application.Services.Product;

public sealed partial class ProductService(
    IProductRepository productRepository,
    IReviewService reviewService,
    IUnitOfWork unitOfWork,
    IEventDispatcher eventDispatcher)
    : IProductService;
