using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Factories;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.PaymentAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.OrderUseCases.CancelOrder;

public class CancelOrderRequestHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IPaymentRepository paymentRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    PaymentFactory paymentFactory
) : IRequestHandler<CancelOrderRequest>
{
    public async Task<Result> Handle(CancelOrderRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);
        var orderId = OrderId.Create(request.OrderId);

        Order? order = await orderRepository.GetByIdAsync(orderId, ct);

        if (order is null || order.Id != orderId)
            return Result.Failure<CancelOrderRequestHandler>(OrderErrors.OrderNotFound(request.OrderId));

        if (order.Status is OrderStatus.Cancelled)
            return Result.Failure(OrderErrors.OrderAlreadyCancelled);

        bool isExpired = order.CreatedAt < DateTimeOffset.UtcNow.AddMinutes(-30);
        bool isValidStatus = order.Status == OrderStatus.PendingPayment || order.Status == OrderStatus.Placed;

        if (!isValidStatus || isExpired)
            return Result.Failure(OrderErrors.CanNotCancel);

        Payment? payment = await paymentRepository.GetByOrderIdAsync(orderId, ct);

        if (payment is not null)
        {
            if (!Enum.TryParse(payment.PaymentMethod, ignoreCase: true, out PaymentMethod paymentMethod))
                return Result.Failure<CancelOrderRequestHandler>(OrderErrors.InvalidPaymentMethod);

            if (payment.Status == PaymentStatus.Paid)
            {
                if (!request.RefundToWallet && paymentMethod is PaymentMethod.Balance)
                    return Result.Failure<CancelOrderRequestHandler>(OrderErrors.RefundFromBalanceToDifferentPaymentProvider);

                if (request.RefundToWallet)
                {
                    User? user = await userRepository.GetByIdAsync(userId, ct);

                    if (user is null)
                        return Result.Failure(UserErrors.NotFound(request.UserId));

                    user.AddToBalance(order.TotalPrice);
                }
                else
                {
                    if (!Enum.TryParse(payment.PaymentProvider, ignoreCase: true, out PaymentProvider provider))
                        return Result.Failure<CancelOrderRequestHandler>(OrderErrors.InvalidPaymentProvider);

                    IPaymentProviderService paymentProvider = paymentFactory.GetProvider(provider);

                    Result refundResult = await paymentProvider.RefundPaymentAsync(
                        payment.TransactionId!,
                        order.TotalPrice.Value,
                        ct
                    );

                    if (refundResult.IsFailure)
                        return Result.Failure(refundResult.Error);
                }

                payment.MarkAsRefunded();
                order.MarkAsRefunded(request.RefundToWallet);
            }
            else
            {
                payment.MarkAsCancelled();
                order.MarkAsCancelled();
            }
        }

        var productIds = order.Items.Select(i => i.ProductId).ToList();
        var products = await productRepository.GetByIdsAsync(productIds, ct);
        var productsDict = products.ToDictionary(p => p.Id);

        foreach (var item in order.Items)
        {
            if (productsDict.TryGetValue(item.ProductId, out var product))
            {
                product.AddStock(item.Quantity);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
