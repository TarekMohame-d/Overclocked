namespace Application.Features.Category.Queries.GetAllCategories;

public record CategoryListDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
