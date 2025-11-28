using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;

namespace Application.Services.Review;

public sealed partial class ReviewService(
    IReviewRepository reviewRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IReviewService;
