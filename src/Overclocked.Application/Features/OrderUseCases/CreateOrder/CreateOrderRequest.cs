using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.OrderUseCases.CreateOrder;

public record CreateOrderRequest : IRequest<CreateOrderResponse>, ICacheInvalidatorRequest
{
    public required Guid UserId { get; init; }
    public required ShippingAddressRequestDto ShippingAddress { get; init; }
    public PaymentMethod PaymentMethod { get; init; }
    public PaymentProvider PaymentProvider { get; init; }

    public string[] CacheKeys => [];
    public string? CacheSetKey => Common.Constants.CacheKeys.OrderSet(UserId.ToString());

    public static Result<CreateOrderRequest> FromDto(CreateOrderRequestDto dto, Guid userId)
    {
        if (!Enum.TryParse(dto.PaymentMethod, ignoreCase: true, out PaymentMethod parsedPaymentMethod))
            return Result.Failure<CreateOrderRequest>(
                Error.Validation(
                    "PaymentMethod",
                    $"Invalid Payment Method Type, Allowed values are: {string.Join(", ", Enum.GetNames(typeof(PaymentMethod)))}"
                )
            );

        if (!Enum.TryParse(dto.PaymentProvider, ignoreCase: true, out PaymentProvider parsedPaymentProvider))
            return Result.Failure<CreateOrderRequest>(
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
            return Result.Failure<CreateOrderRequest>(
                Error.Validation(
                    "PaymentMethod",
                    $"payment provider: '{parsedPaymentProvider}' does not support payment method: '{parsedPaymentMethod}'"
                )
            );
        }

        var request = new CreateOrderRequest
        {
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            PaymentMethod = parsedPaymentMethod,
            PaymentProvider = parsedPaymentProvider,
        };

        return Result.Success(request);
    }
}
