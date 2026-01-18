using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ProductUseCases.GetPagedProducts;

public class GetPagedProductsRequestHandler(IProductReadRepository productRepository)
    : IRequestHandler<GetPagedProductsRequest, PagedResult<ProductPagedResponse>>
{
    public async Task<Result<PagedResult<ProductPagedResponse>>> Handle(GetPagedProductsRequest request, CancellationToken ct)
    {
        var totalCount = await productRepository.CountAsync(
            request.SearchTerm,
            request.BrandId,
            request.CategoryId,
            request.TagId,
            request.HasDiscount,
            ct
        );

        if (totalCount == 0)
            return Result.Success(PagedResult<ProductPagedResponse>.Empty(request.Page, request.PageSize));

        List<ProductPagedResponse> dtos = await productRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.BrandId,
            request.CategoryId,
            request.TagId,
            request.ProductSortField,
            request.SortDirection,
            request.HasDiscount,
            selector: p => new ProductPagedResponse
            {
                Id = p.Id.Value,
                Name = p.Name,
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
            },
            ct
        );

        return Result.Success(PagedResult<ProductPagedResponse>.Create(dtos, request.Page, request.PageSize, totalCount));
    }
}
