namespace Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;

public record CategoryListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
