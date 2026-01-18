namespace Overclocked.Application.Features.TagUseCases.DTOs.Responses;

public record TagResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
