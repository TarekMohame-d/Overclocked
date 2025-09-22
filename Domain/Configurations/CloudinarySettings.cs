namespace Domain.Configurations;

public sealed class CloudinarySettings
{
    public required string CloudName { get; set; } = string.Empty;
    public required string ApiKey { get; set; } = string.Empty;
    public required string ApiSecret { get; set; } = string.Empty;
}
