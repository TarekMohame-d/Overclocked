namespace Application.Features.Tag.Queries.GetAllTags;

public record TagListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
