namespace Application.Services.Tag.DTOs.Request;

public record UpdateTagRequestBody
{
    public required string Name { get; init; }
}

public record UpdateTagRequest : UpdateTagRequestBody
{
    public required Guid Id { get; init; }
}
