using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Factories;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.Common.Shared.ValueObjects.Address;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;
using Polly;
using Polly.Registry;

namespace Overclocked.Application.Features.OrderUseCases.CreateOrder;

public class CreateOrderRequestHandler(
    IOrderRepository orderRepository,
    IUserRepository userRepository,
    IProductRepository productRepository,
    ICartRepository cartRepository,
    IPaymentRepository paymentRepository,
    PaymentFactory paymentFactory,
    IUnitOfWork unitOfWork,
    ResiliencePipelineProvider<string> pipelineProvider
) : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
{
    public async Task<Result<CreateOrderResponse>> Handle(CreateOrderRequest request, CancellationToken ct)
    {
        var userId = UserId.Create(request.UserId);

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

        ResiliencePipeline pipeline = pipelineProvider.GetPipeline(ResilienceConstants.StandardPolicy);

        return await pipeline.ExecuteAsync(
            async token =>
            {
                unitOfWork.ClearChangeTracker();

                Cart cart = await cartRepository.GetAsync(userId, token) ?? throw new CartNotFoundException(request.UserId);

                if (!cart.CartItems.Any())
                    return Result.Failure<CreateOrderResponse>(OrderErrors.EmptyCart);

                var productIds = cart.CartItems.Select(x => x.ProductId);
                List<Product> products = await productRepository.GetByIdsAsync([.. productIds], token);
                var productsDict = products.ToDictionary(p => p.Id);

                var order = Order.Create(UserId.Create(request.UserId), address.Value);

                foreach (var cartItem in cart.CartItems)
                {
                    if (!productsDict.TryGetValue(cartItem.ProductId, out Product? product))
                        return Result.Failure<CreateOrderResponse>(ProductErrors.ProductNotFound(cartItem.ProductId.Value));

                    Result stockResult = product.RemoveStock(cartItem.Quantity);
                    if (stockResult.IsFailure)
                        return Result.Failure<CreateOrderResponse>(stockResult.Error);

                    Money price = product.CalculateFinalPrice();
                    order.AddItem(product.Id, product.Name, product.Thumbnail, price.Value, cartItem.Quantity);
                }

                order.CalculateTotalPrice();

                var payment = Payment.Create(
                    order.Id,
                    request.PaymentProvider.ToString(),
                    request.PaymentMethod.ToString(),
                    order.TotalPrice
                );

                User? user = await userRepository.GetByIdAsync(userId, token);

                string? redirectUrl = null;
                var paymentPending = false;

                if (request.PaymentMethod == PaymentMethod.Balance)
                {
                    if (user!.Balance >= order.TotalPrice)
                    {
                        user.RemoveFromBalance(order.TotalPrice);
                        order.MarkAsPlaced(isBalance: true);
                        payment.MarkAsPaid();
                    }
                    else
                    {
                        return Result.Failure<CreateOrderResponse>(OrderErrors.InsufficientBalance);
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

                orderRepository.Add(order);
                paymentRepository.Add(payment);
                cart.Clear();

                await unitOfWork.SaveChangesAsync(token);

                return Result.Success(
                    new CreateOrderResponse
                    {
                        OrderId = order.Id.Value,
                        RedirectUrl = redirectUrl,
                        PaymentPending = paymentPending,
                    }
                );
            },
            ct
        );
    }
}
