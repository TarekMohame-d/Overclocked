using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Category.DTOs.Response;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Application.Services.Tag.DTOs.Response;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Product;

public sealed partial class ProductService
{
    public async Task<Result<ProductResponse>> GetProductByIdAsync(
        GetProductByIdRequest request,
        CancellationToken cancellationToken)
    {
        ProductResponse? productResponse = await productRepository
            .Query()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Thumbnail = p.Thumbnail,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                Rating = p.Rating,
                Category = new CategoryResponse
                {
                    Id = p.Category!.Id,
                    Name = p.Category.Name,
                    ImageUrl = p.Category.Image,
                },
                Brand = new BrandResponse
                {
                    Id = p.Brand!.Id,
                    Name = p.Brand.Name,
                    ImageUrl = p.Brand.Image,
                },
                Tags = p.TagProducts.Select(tp => new TagResponse
                {
                    Id = tp.Tag!.Id,
                    Name = tp.Tag.Name
                }),
                Specifications = p.Specifications.Select(s => new ProductSpecificationResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    Value = s.Value,
                }),
                Images = p.ProductImages.Select(i => i.Image),
            })
            .SingleOrDefaultAsync(cancellationToken);

        return productResponse is null
            ? Result<ProductResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound)
            : Result<ProductResponse>.Success(productResponse);
    }
}
