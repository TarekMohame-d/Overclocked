using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.ProductUseCases.DTOs.Requests;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ProductUseCases.UpdateProduct;

public record UpdateProductRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid Id { get; init; }
    public required Guid BrandId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int StockQuantity { get; init; }
    public decimal? Discount { get; init; }
    public required List<Guid> Tags { get; init; }
    public required List<ProductSpecificationDto> Specifications { get; init; }
    public List<string>? Images { get; init; }
    public bool IsDeleted { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Product(Id.ToString())];
    public string? CacheSetKey => Common.Constants.CacheKeys.ProductSet;

    public static UpdateProductRequest FromDto(UpdateProductRequestDto dto, Guid id) =>
        new()
        {
            Id = id,
            BrandId = dto.BrandId,
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Thumbnail = dto.Thumbnail,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Discount = dto.Discount,
            Tags = dto.Tags,
            Specifications = dto.Specifications,
            Images = dto.Images,
            IsDeleted = dto.IsDeleted,
        };
}
