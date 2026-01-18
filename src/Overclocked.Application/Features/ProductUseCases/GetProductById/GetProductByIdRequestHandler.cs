using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ProductUseCases.GetProductById;

public class GetProductByIdRequestHandler(IProductReadRepository productRepository, IReviewReadRepository reviewRepository)
    : IRequestHandler<GetProductByIdRequest, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdRequest request, CancellationToken ct)
    {
        var productId = ProductId.Create(request.Id);

        ProductResponse? productDto = await productRepository.GetByIdAsync(
            productId,
            selector: p => new ProductResponse
            {
                Id = p.Id.Value,
                Name = p.Name,
                Description = p.Description,
                Thumbnail = p.Thumbnail.Value,
                Price = p.Price.Value,
                Discount = p.Discount.Value,
                Rating = p.ProductRating.AverageRating,
                ReviewCount = p.ProductRating.ReviewCount,

                Brand = new BrandResponse
                {
                    Id = p.Brand!.Id.Value,
                    Name = p.Brand.Name,
                    ImageUrl = p.Brand.Image.Value,
                },

                Category = new CategoryResponse
                {
                    Id = p.Category!.Id.Value,
                    Name = p.Category.Name,
                    ImageUrl = p.Category.Image.Value,
                },

                Images = p.Images.Select(i => i.Image.Value),

                Specifications = p.Specifications.Select(s => new ProductSpecificationDto { Name = s.Name, Value = s.Value }),

                Tags = p.ProductTags.Select(pt => new TagResponse { Id = pt.Tag!.Id.Value, Name = pt.Tag.Name }),

                RatingsBreakdown = new Dictionary<int, int>(),
            },
            ct
        );

        if (productDto is null)
            return Result.Failure<ProductResponse>(ProductErrors.ProductNotFound(request.Id));

        IDictionary<int, int> ratingsBreakdown = await reviewRepository.GetProductRatingsBreakdownAsync(productId, ct);

        return Result.Success(productDto with { RatingsBreakdown = ratingsBreakdown });
    }
}
