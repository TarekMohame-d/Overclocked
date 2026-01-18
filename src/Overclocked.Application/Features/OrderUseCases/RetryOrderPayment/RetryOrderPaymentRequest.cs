using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.OrderUseCases.RetryOrderPayment;

public record RetryOrderPaymentRequest : IRequest<CreateOrderResponse>, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }
    public required Guid OrderId { get; init; }
    public required ShippingAddressRequestDto ShippingAddress { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public PaymentProvider PaymentProvider { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.OrderSet(UserId.ToString());

    public static Result<RetryOrderPaymentRequest> FromDto(RetryOrderPaymentRequestDto dto, Guid userId, Guid orderId)
    {
        if (!Enum.TryParse(dto.PaymentMethod, ignoreCase: true, out PaymentMethod parsedPaymentMethod))
            return Result.Failure<RetryOrderPaymentRequest>(
                Error.Validation(
                    "PaymentMethod",
                    $"Invalid Payment Method Type, Allowed values are: {string.Join(", ", Enum.GetNames(typeof(PaymentMethod)))}"
                )
            );

        if (!Enum.TryParse(dto.PaymentProvider, ignoreCase: true, out PaymentProvider parsedPaymentProvider))
            return Result.Failure<RetryOrderPaymentRequest>(
                Error.Validation(
                    "PaymentProvider",
                    $"Invalid Payment Provider Type, Allowed values are: {string.Join(", ", Enum.GetNames(typeof(PaymentProvider)))}"
                )
            );

        (PaymentProvider Provider, List<PaymentMethod> Methods) validConfiguration = PaymentProviders.AllProviders.FirstOrDefault(
            p => p.Provider == parsedPaymentProvider
        );

        if (validConfiguration == default || !validConfiguration.Methods.Contains(parsedPaymentMethod))
        {
            return Result.Failure<RetryOrderPaymentRequest>(
                Error.Validation(
                    "PaymentMethod",
                    $"payment provider: '{parsedPaymentProvider}' does not support payment method: '{parsedPaymentMethod}'"
                )
            );
        }

        var request = new RetryOrderPaymentRequest
        {
            UserId = userId,
            OrderId = orderId,
            ShippingAddress = dto.ShippingAddress,
            PaymentMethod = parsedPaymentMethod,
            PaymentProvider = parsedPaymentProvider,
        };

        return Result.Success(request);
    }
}
