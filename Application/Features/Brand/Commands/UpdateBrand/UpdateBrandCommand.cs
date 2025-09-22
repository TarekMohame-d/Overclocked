using Application.Abstraction.Messaging;
using Application.Common.Results;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Brand.Commands.UpdateBrand;

public record UpdateBrandWithIdCommand : UpdateBrandCommand, ICommand<Result>
{
    public Guid Id { get; init; }
}

public record UpdateBrandCommand
{
    public required string Name { get; init; }
    public IFormFile? ImageFile { get; init; }
    public string? ImageUrl { get; init; }
}
