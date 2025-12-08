namespace Overclocked.Infrastructure.Configurations;

public sealed class CloudinarySettings
{
    public const string SectionName = "CloudinarySettings";

    public required string CloudName { get; set; }
    public required string ApiKey { get; set; }
    public required string ApiSecret { get; set; }
}
