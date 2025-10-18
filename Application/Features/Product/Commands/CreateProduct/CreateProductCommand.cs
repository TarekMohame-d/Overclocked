using System.Data;
using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Product.Commands.CreateProduct;

public record CreateProductCommand : CreateProductRequest, ICommand<Result>, ITransactionalRequest
{
    public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
}


public record CreateProductRequest
{
    public Guid BrandId { get; init; }
    public Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public decimal Discount { get; init; }
    public IEnumerable<Guid> Tags { get; init; } = [];
    public IEnumerable<string>? Images { get; init; }
    public IEnumerable<Specs> Specification { get; init; } = [];
}

public record Specs
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}
