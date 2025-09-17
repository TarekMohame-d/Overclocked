namespace Domain.StaticData;

public enum InvoiceStatusType
{
    Paid = 1,           // Payment completed successfully and confirmed.
    Refunded,           // Payment was refunded after cancellation or return.
    PartiallyRefunded,  // Some items have been refunded, the rest are still processing.
}
