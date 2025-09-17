namespace Domain.StaticData;

public enum ShipmentStatusType
{
    Pending = 1,    // Order has been placed but shipping has not started yet.
    Processing,     // Shipping team is preparing the package (e.g., packaging, labeling).
    Shipped,        // Package has left the warehouse and is on the way to the destination.
    OutForDelivery, // Package is with the delivery agent and will be delivered soon.
    Delivered,      // Package has been successfully delivered to the customer.
    Returned,       // Package has been returned to the warehouse.
    Cancelled,      // Shipment was cancelled before dispatching.
    Failed          // Delivery attempt failed (e.g., customer not available).
}
