namespace Application.Features.Product.Commands.Common.DTOs;

public class SpecificationDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Value { get; init; }
}
