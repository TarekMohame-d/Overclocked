using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Overclocked.Infrastructure.Services.PaymentService.Strategies.Paymob;

internal sealed class PaymobValidator
{
    public static bool ValidateProcessedCallback(string hmacSecret, JsonElement payload, string receivedHmac)
    {
        // Define the keys in the exact lexicographical order provided by Paymob
        var concatenatedString =
            GetVal(payload, "amount_cents")
            + GetVal(payload, "created_at")
            + GetVal(payload, "currency")
            + GetVal(payload, "error_occured")
            + GetVal(payload, "has_parent_transaction")
            + GetVal(payload, "id")
            + GetVal(payload, "integration_id")
            + GetVal(payload, "is_3d_secure")
            + GetVal(payload, "is_auth")
            + GetVal(payload, "is_capture")
            + GetVal(payload, "is_refunded")
            + GetVal(payload, "is_standalone_payment")
            + GetVal(payload, "is_voided")
            + GetVal(payload, "order", "id")
            + GetVal(payload, "owner")
            + GetVal(payload, "pending")
            + GetVal(payload, "source_data", "pan")
            + GetVal(payload, "source_data", "sub_type")
            + GetVal(payload, "source_data", "type")
            + GetVal(payload, "success");

        var calculatedHmac = CalculateHmacSha512(concatenatedString, hmacSecret);

        return string.Equals(calculatedHmac, receivedHmac, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetVal(JsonElement element, params string[] keys)
    {
        JsonElement current = element;
        foreach (var key in keys)
        {
            if (current.ValueKind == JsonValueKind.Object && current.TryGetProperty(key, out JsonElement next))
            {
                current = next;
            }
            else
            {
                return string.Empty;
            }
        }

        // formatting rules for Paymob:
        return current.ValueKind switch
        {
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "",
            JsonValueKind.Undefined => "",
            JsonValueKind.Number => current.ToString(),
            JsonValueKind.String => current.GetString() ?? "",
            _ => current.ToString(),
        };
    }

    private static string CalculateHmacSha512(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
