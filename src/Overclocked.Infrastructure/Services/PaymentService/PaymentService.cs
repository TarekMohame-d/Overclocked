using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Services.PaymentService;

public class PaymentService(
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    ILogger<PaymentService> logger
) : IPaymentService
{
    public async Task HandleSuccessfulPaymentAsync(OrderId orderId, UserId userId, string transactionId)
    {
        logger.LogInformation(
            "Payment succeeded, moving order with id {OrderId} to Placed state. User {UserId}, TransactionId {TransactionId}",
            orderId.Value,
            userId.Value,
            transactionId
        );

        Order? order = await orderRepository.GetByIdAsync(orderId);

        Payment? payment = await paymentRepository.GetByOrderIdAsync(orderId);

        payment?.MarkAsPaid(transactionId);

        order?.MarkAsPlaced();

        await unitOfWork.SaveChangesAsync();
    }

    public async Task HandleFailedPaymentAsync(OrderId orderId, UserId userId, string transactionId)
    {
        logger.LogInformation(
            "Payment failed, moving order with id {OrderId} to Failed state. User {UserId}, TransactionId {TransactionId}",
            orderId.Value,
            userId.Value,
            transactionId
        );

        Payment? payment = await paymentRepository.GetByOrderIdAsync(orderId);

        payment?.MarkAsFailed(orderId.Value);

        await unitOfWork.SaveChangesAsync();
    }

    public async Task HandleRefundAsync(OrderId orderId, UserId userId, string transactionId, Money amount)
    {
        logger.LogInformation(
            "Refund payment for order with id {OrderId}. User {UserId}, TransactionId {TransactionId}",
            orderId.Value,
            userId.Value,
            transactionId
        );

        Order? order = await orderRepository.GetByIdAsync(orderId);

        order?.MarkAsRefunded(false);

        Payment? payment = await paymentRepository.GetByOrderIdAsync(orderId);

        payment?.MarkAsRefunded();

        await unitOfWork.SaveChangesAsync();
    }
}
