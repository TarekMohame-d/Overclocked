namespace Domain.StaticData;

public enum OrderStatusType
{
    Pending = 1, // Order has been placed but payment is not confirmed yet.
    Confirmed, // Payment is confirmed, and order is approved for processing.
    Processing, // Order items are being prepared (e.g., packing, inventory check).
    ReadyForShipping, // Order is fully packed and ready to be handed over to the shipping team.
    Shipped, // Order has been handed over to the courier for delivery.
    PartiallyShipped, // Some items have shipped, the rest are still processing.
    Delivered, // Order has been successfully delivered to the customer.
    PartiallyDelivered, // Some items have been delivered, the rest are still processing.
    Cancelled, // Order has been cancelled by the customer or system.
    Returned, // Customer returned the order after delivery.
    PartiallyReturned, // Some items have been returned, the rest are still processing.
    Refunded, // Payment was refunded after cancellation or return.
    PartiallyRefunded, // Some items have been refunded, the rest are still processing.
    Failed, // Order failed due to payment failure or other critical issue.
}
