namespace Application.Services.Tag.DTOs.Request;

public record DeleteTagRequest
{
    public required Guid Id { get; init; }
}
