using System.Text.Json.Serialization;

namespace Application.Services.Category.DTOs.Request;

public record UpdateCategoryRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
