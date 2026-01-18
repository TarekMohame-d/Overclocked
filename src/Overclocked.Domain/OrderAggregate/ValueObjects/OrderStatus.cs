namespace Overclocked.Domain.OrderAggregate.ValueObjects;

public enum OrderStatus
{
    PendingPayment = 1, // Order has been placed but payment is not confirmed yet.
    Placed, // Order is approved, has been placed and wait for Grace Period to confirm.
    Processing, // Order items are being prepared (e.g., packing, inventory check).
    ReadyForShipping, // Order is fully packed and ready to be handed over to the shipping team.
    Shipped, // Order has been handed over to the courier for delivery.
    Delivered, // Order has been successfully delivered to the customer.
    Cancelled, // Order has been cancelled by the customer or system.
    Returned, // Customer returned the order after delivery.
    Refunded, // Payment was refunded after cancellation or return.
}
