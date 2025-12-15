using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Events;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Review.Commands.EventHandlers;

public class ReviewUpdatedUpdateProductReviewEventHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICacheService cacheService) : IDomainEventHandler<ReviewUpdatedEvent>
{
    public async Task Handle(ReviewUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ProductEntity? product = await productRepository.FetchPrimitiveAsync(
            ProductId.Create(domainEvent.ProductId),
            cancellationToken);

        if(product is null)
        {
            return;
        }

        product.UpdateReviewVote(domainEvent.OldRating, domainEvent.NewRating);

        productRepository.Update(product);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch(DbUpdateConcurrencyException)
        {
            throw;
        }

        await cacheService.RemoveAsync(CacheKeys.Product(domainEvent.ProductId.ToString()), cancellationToken);
    }
}
