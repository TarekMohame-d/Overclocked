using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Contracts.Review;

namespace Overclocked.Application.Review.Queries.GetProductRatingBreakdown;

public record GetProductRatingBreakdownQuery : IQuery<RatingBreakdownResponse>
{
    public required Guid ProductId { get; init; }
}
