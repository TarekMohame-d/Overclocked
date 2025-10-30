using System.Text.Json.Serialization;

namespace Application.Services.Brand.DTOs.Request;

public record UpdateBrandRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
