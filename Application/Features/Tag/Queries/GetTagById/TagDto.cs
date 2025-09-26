namespace Application.Features.Tag.Queries.GetTagById;

public record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
