using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Factories;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.Domain.Common.Shared.ValueObjects.Address;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.OrderUseCases.RetryOrderPayment;

public class RetryOrderPaymentRequestHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IPaymentRepository paymentRepository,
    IUnitOfWork unitOfWork,
    PaymentFactory paymentFactory
) : IRequestHandler<RetryOrderPaymentRequest, CreateOrderResponse>
{
    public async Task<Result<CreateOrderResponse>> Handle(RetryOrderPaymentRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);
        var orderId = OrderId.Create(request.OrderId);

        Result<Address> address = Address.Create(
            request.ShippingAddress.Apartment,
            request.ShippingAddress.Building,
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.PostalCode,
            request.ShippingAddress.Description
        );

        if (address.IsFailure)
            return Result.Failure<CreateOrderResponse>(address.Error);

        Order? order = await orderRepository.GetByIdAsync(orderId, ct);

        if (order is null)
            return Result.Failure<CreateOrderResponse>(OrderErrors.OrderNotFound(request.OrderId));

        if (order.UserId != userId)
            return Result.Failure<CreateOrderResponse>(OrderErrors.OrderDoesNotBelongToUser);

        if (order.Status is not OrderStatus.PendingPayment)
            return Result.Failure<CreateOrderResponse>(OrderErrors.NotInPendingPaymentState);

        var timeSinceCreation = DateTimeOffset.UtcNow - order.CreatedAt;
        if (timeSinceCreation.TotalMinutes > 30)
        {
            return Result.Failure<CreateOrderResponse>(OrderErrors.Expired);
        }

        order.UpdateShippingAddress(address.Value);
        User? user = await userRepository.GetByIdAsync(userId, ct);
        Payment? payment = await paymentRepository.GetByOrderIdAsync(orderId, ct);

        string? redirectUrl = null;
        var paymentPending = false;

        if (request.PaymentMethod == PaymentMethod.Balance)
        {
            if (user!.Balance >= order.TotalPrice)
            {
                user.RemoveFromBalance(order.TotalPrice);
                order.MarkAsPlaced(isBalance: true);
                payment!.MarkAsPaid();
            }
            else
            {
                payment!.MarkAsFailed(order.Id.Value);
                return Result.Failure<CreateOrderResponse>(
                    Error.BadRequest("Order.InsufficientBalance", "Insufficient balance.")
                );
            }
        }
        else if (request.PaymentMethod == PaymentMethod.CashOnDelivery)
        {
            order.MarkAsPlaced(isCod: true);
        }
        else
        {
            IPaymentProviderService provider = paymentFactory.GetProvider(request.PaymentProvider);

            Result<string> paymentUrlResult = await provider.GeneratePaymentUrl(order, user!, request.PaymentMethod, ct);

            if (paymentUrlResult.IsFailure)
                return Result.Failure<CreateOrderResponse>(paymentUrlResult.Error);

            redirectUrl = paymentUrlResult.Value;
            paymentPending = true;
        }

        payment!.UpdatePaymentInfo(request.PaymentProvider.ToString(), request.PaymentMethod.ToString());
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(
            new CreateOrderResponse
            {
                OrderId = order.Id.Value,
                RedirectUrl = redirectUrl,
                PaymentPending = paymentPending,
            }
        );
    }
}
