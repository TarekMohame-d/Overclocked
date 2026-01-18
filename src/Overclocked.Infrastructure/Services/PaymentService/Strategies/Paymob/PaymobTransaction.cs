using System.Text.Json.Serialization;

namespace Overclocked.Infrastructure.Services.PaymentService.Strategies.Paymob;

// The root object from the webhook
public class PaymobCallbackRoot
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("obj")]
    public PaymobTransaction Obj { get; set; } = new();
}

// The main transaction object ("obj")
public class PaymobTransaction
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("amount_cents")]
    public int AmountCents { get; set; }

    [JsonPropertyName("is_refunded")]
    public bool IsRefunded { get; set; }

    [JsonPropertyName("is_voided")]
    public bool IsVoided { get; set; }

    [JsonPropertyName("pending")]
    public bool Pending { get; set; }

    [JsonPropertyName("payment_key_claims")]
    public PaymobClaims? Claims { get; set; }

    public string TransactionId => Id.ToString();
}

public class PaymobClaims
{
    [JsonPropertyName("extra")]
    public PaymobExtras? Extra { get; set; }
}

// The custom data sent during GeneratePaymentUrl
public class PaymobExtras
{
    [JsonPropertyName("order_id")]
    public Guid? OrderId { get; set; }

    [JsonPropertyName("customer_id")]
    public Guid? CustomerId { get; set; }

    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; set; }
}
