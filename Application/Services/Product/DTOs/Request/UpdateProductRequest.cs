namespace Application.Services.Product.DTOs.Request;

public record UpdateProductRequestBody
{
    public required Guid BrandId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int Stock { get; init; }
    public required decimal Discount { get; init; }
    public required IEnumerable<Guid> Tags { get; init; }
    public IEnumerable<string>? Images { get; init; }
    public required IEnumerable<Specs> Specification { get; init; }

    public record Specs
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
    }
}

public record UpdateProductRequest : UpdateProductRequestBody
{
    public required Guid Id { get; init; }

    public static UpdateProductRequest FromBody(UpdateProductRequestBody request, Guid id)
    {
        return new UpdateProductRequest
        {
            Id = id,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Thumbnail = request.Thumbnail,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            Discount = request.Discount,
            Tags = request.Tags,
            Specification = request.Specification,
            Images = request.Images
        };
    }
}
