using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.PaymentUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.PaymentUseCases.GetPayments;

public record GetPaymentsRequest : IRequest<PaymentResponse>, ICachedRequest
{
    public string CacheKey => CacheKeys.PaymentMethods;
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(30);
}
