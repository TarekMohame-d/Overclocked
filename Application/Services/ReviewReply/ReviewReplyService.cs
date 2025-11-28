using Application.Abstraction.DomainServices;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;

namespace Application.Services.ReviewReply;

public sealed partial class ReviewReplyService(
    IGenericRepository<Domain.Entities.ReviewReply> reviewReplyRepository,
    IReviewRepository reviewRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService)
    : IReviewReplyService;
