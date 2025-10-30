namespace Application.Services.Tag.DTOs.Response;

public record TagResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
