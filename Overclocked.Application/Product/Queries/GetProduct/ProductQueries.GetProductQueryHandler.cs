using System.Net;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Contracts.Brand;
using Overclocked.Contracts.Category;
using Overclocked.Contracts.Product;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Queries;

public sealed partial class ProductQueries
{
    public async Task<Result<ProductResponse>> GetProductQueryHandler(
        GetProductQuery query,
        CancellationToken cancellationToken)
    {
        ProductEntity? p = await productRepository.GetByIdWithDetailsAsync(
            ProductId.Create(query.Id),
            cancellationToken);

        if(p is null)
        {
            return Result<ProductResponse>.Failure(ProductErrors.ProductNotFound(query.Id), HttpStatusCode.NotFound);
        }

        IDictionary<int, int> ratingsBreakdown = await reviewRepository
            .GetProductRatingsBreakdownAsync(ProductId.Create(query.Id), cancellationToken);

        var productResponse = new ProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Thumbnail = p.Thumbnail,
            Price = p.Price.Amount,
            Discount = p.Discount.Amount,
            FinalPrice = p.CalculateFinalPrice(),
            Rating = p.ProductRating.Rating,
            ReviewCount = p.ProductRating.ReviewCount,

            Brand = new BrandResponse
            {
                Id = p.Brand!.Id.Value,
                Name = p.Brand.Name,
                ImageUrl = p.Brand.ImageUrl
            },

            Category = new CategoryResponse
            {
                Id = p.Category!.Id.Value,
                Name = p.Category.Name,
                ImageUrl = p.Category.ImageUrl
            },

            Images = p.Images.Select(i => i.ImageUrl),

            Specifications = p.Specifications.Select(s => new ProductSpecificationDto
            {
                Name = s.Name,
                Value = s.Value
            }),

            Tags = p.Tags.Select(pt => new TagResponse
            {
                Id = pt.Tag!.Id.Value,
                Name = pt.Tag.Name
            }),

            RatingsBreakdown = ratingsBreakdown
        };

        return Result<ProductResponse>.Success(productResponse);
    }
}
