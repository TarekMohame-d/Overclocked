using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ReviewUseCases.GetProductRatingBreakdown;

public record GetProductRatingBreakdownRequest : IRequest<RatingBreakdownResponse>
{
    public required Guid ProductId { get; init; }
}
