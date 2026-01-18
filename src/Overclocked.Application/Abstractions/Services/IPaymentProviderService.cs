using Microsoft.AspNetCore.Http;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Abstractions.Services;

public interface IPaymentProviderService
{
    PaymentProvider PaymentProvider { get; }
    Task<Result<string>> GeneratePaymentUrl(Order order, User user, PaymentMethod method, CancellationToken ct);
    Task<Result> ProcessCallback(string rawBody, IHeaderDictionary headers, IQueryCollection queryParams);
    Task<Result> RefundPaymentAsync(string transactionId, decimal refundValue, CancellationToken ct);
}
