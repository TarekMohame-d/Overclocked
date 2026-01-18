using Overclocked.SharedKernel;

namespace Overclocked.Domain.OrderAggregate;

public static class OrderErrors
{
    public static Error OrderNotFound(Guid id) => Error.NotFound("Order.NotFound", $"The Order with Id: '{id}' was not found.");

    public static Error OrderNotPending =>
        Error.Conflict("Order.NotPending", "You cannot add or remove items to an order that is not pending.");

    public static Error EmptyCart => Error.BadRequest("Order.EmptyCart", "Cannot place an order with an empty cart.");

    public static Error InsufficientBalance => Error.BadRequest("Order.InsufficientBalance", "Insufficient balance.");

    public static Error InvalidOrderItemQuantity =>
        Error.Validation("Order.InvalidOrderItemQuantity", "Order item quantity must be greater than 0.");

    public static Error NotInPendingPaymentState =>
        Error.BadRequest("Order.NotInPendingPaymentState", "The order is already placed or cancelled.");

    public static Error CanNotCancel =>
        Error.BadRequest(
            "Order.CanNotCancel",
            "The order is processing or out for delivery, you can make a return request later."
        );

    public static Error OrderAlreadyCancelled =>
        Error.BadRequest("Order.AlreadyCancelled", "The order has already been cancelled.");

    public static Error Expired =>
        Error.BadRequest("Order.Expired", "This order has expired and can no longer be paid for. Please create a new order.");

    public static readonly Error OrderDoesNotBelongToUser = new(
        "Order.Unauthorized",
        "You are not authorized to perform this action.",
        ErrorType.Unauthorized
    );

    public static Error InvalidPaymentProvider =>
        Error.BadRequest("Order.InvalidPaymentProvider", "The payment provider is invalid.");

    public static Error InvalidPaymentMethod => Error.BadRequest("Order.InvalidPaymentMethod", "The payment method is invalid.");

    public static Error RefundFromBalanceToDifferentPaymentProvider =>
        Error.BadRequest(
            "Order.RefundFromBalanceToDifferentPaymentProvider",
            "Can not refund from balance to different payment provider."
        );

    // OrderItem errors
    public static Error OrderItemInvalidProductName =>
        Error.Validation("OrderItem.ProductName", "Product name is required, and must be less than 50 characters.");

    public static Error OrderItemInvalidUnitPrice =>
        Error.Validation("OrderItem.UnitPrice", "Unit price must be greater than 0.");

    public static Error OrderItemInvalidQuantity => Error.Validation("OrderItem.Quantity", "Quantity must be greater than 0.");
}
