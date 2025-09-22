using Application.Abstraction.Messaging;
using Application.Common.Results;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Brand.Commands.CreateBrand;

public record CreateBrandCommand : ICommand<Result>
{
    public required string Name { get; init; }
    public required IFormFile ImageFile { get; init; }
}
