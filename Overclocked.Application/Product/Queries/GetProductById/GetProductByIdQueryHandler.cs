using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Contracts.Brand;
using Overclocked.Contracts.Category;
using Overclocked.Contracts.Product;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Queries.GetProductById;

public class GetProductByIdQueryHandler(
    IProductRepository productRepository,
    IReviewRepository reviewRepository) : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        ProductEntity? p = await productRepository.GetByIdAsync(ProductId.Create(query.Id), cancellationToken);

        if(p is null)
        {
            return Result.Failure<ProductResponse>(ProductErrors.ProductNotFound(query.Id));
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
            Rating = p.ProductRating.AverageRating,
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

        return Result.Success(productResponse);
    }
}
