using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Abstractions.Services;

public interface IPaymentService
{
    Task HandleSuccessfulPaymentAsync(OrderId orderId, UserId userId, string transactionId);
    Task HandleFailedPaymentAsync(OrderId orderId, UserId userId, string transactionId);
    Task HandleRefundAsync(OrderId orderId, UserId userId, string transactionId, Money amount);
}
