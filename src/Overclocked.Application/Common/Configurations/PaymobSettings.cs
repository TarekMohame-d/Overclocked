namespace Overclocked.Application.Common.Configurations;

public sealed class PaymobSettings
{
    public const string SectionName = "PaymobSettings";

    public string BaseUrl { get; set; } = string.Empty;
    public string Hmac { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string PublicKey { get; set; } = string.Empty;
    public int CardIntegrationId { get; set; }
    public int EWalletIntegrationId { get; set; }
}
