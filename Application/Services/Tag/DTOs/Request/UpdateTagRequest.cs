using System.Text.Json.Serialization;

namespace Application.Services.Tag.DTOs.Request;

public record UpdateTagRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public required string Name { get; init; }
}
