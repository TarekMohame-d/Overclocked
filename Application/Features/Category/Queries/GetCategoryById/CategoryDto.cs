namespace Application.Features.Category.Queries.GetCategoryById;

public record CategoryDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
