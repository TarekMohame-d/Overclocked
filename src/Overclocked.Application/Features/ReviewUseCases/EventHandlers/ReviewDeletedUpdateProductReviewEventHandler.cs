using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Events;
using Overclocked.SharedKernel.Primitives;
using Polly;
using Polly.Registry;

namespace Overclocked.Application.Features.ReviewUseCases.EventHandlers;

public class ReviewDeletedUpdateProductReviewEventHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService,
    ResiliencePipelineProvider<string> pipelineProvider
) : IDomainEventHandler<ReviewDeletedEvent>
{
    public async Task Handle(ReviewDeletedEvent domainEvent, CancellationToken ct = default)
    {
        ResiliencePipeline pipeline = pipelineProvider.GetPipeline(ResilienceConstants.StandardPolicy);

        await pipeline.ExecuteAsync(
            async token =>
            {
                unitOfWork.ClearChangeTracker();

                Product? product = await productRepository.FindAsync(ProductId.Create(domainEvent.ProductId), token);

                if (product is null)
                    return;

                product.RemoveReviewVote(domainEvent.Rating);

                await unitOfWork.SaveChangesAsync(token);
            },
            ct
        );

        await cacheService.RemoveAsync(CacheKeys.Product(domainEvent.ProductId.ToString()), ct);
    }
}
