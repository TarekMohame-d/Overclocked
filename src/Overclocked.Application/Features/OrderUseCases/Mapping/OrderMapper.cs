using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;

namespace Overclocked.Application.Features.OrderUseCases.Mapping;

public static class OrderMapper
{
    public static List<OrderPagedResponse> ToDto(this List<Order> orders)
    {
        return orders
            .Select(order => new OrderPagedResponse
            {
                OrderId = order.Id.Value,
                OrderNumber = order.OrderNumber,
                OrderDate = order.CreatedAt,
                CanBeCancelled =
                    (order.Status is OrderStatus.PendingPayment or OrderStatus.Placed)
                    && order.CreatedAt > DateTimeOffset.UtcNow.AddMinutes(-30),
                OrderStatus = order.Status.ToString(),
                OrderItems = order
                    .Items.Select(item => new OrderItemResponse
                    {
                        OrderItemId = item.Id.Value,
                        ProductId = item.ProductId.Value,
                        ProductName = item.ProductName,
                        ProductThumbnail = item.ProductImage.Value,
                        UnitPrice = item.UnitPrice.Value,
                        Quantity = item.Quantity,
                    })
                    .ToList(),
                Total = order.TotalPrice.Value,
            })
            .ToList();
    }
}
