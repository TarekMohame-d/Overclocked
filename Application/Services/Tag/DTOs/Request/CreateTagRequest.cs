namespace Application.Services.Tag.DTOs.Request;

public record CreateTagRequest
{
    public required string Name { get; init; }
}
