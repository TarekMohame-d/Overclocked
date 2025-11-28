using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Category.DTOs.Response;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Application.Services.Review.DTOs.Response;
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
                FinalPrice = Math.Round(p.Price * (1 - p.Discount), 2),
                Discount = p.Discount,
                Rating = p.Rating,
                ReviewCount = p.ReviewCount,
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
                Reviews = p.Reviews
                .OrderByDescending(r => r.UpdatedAt)
                .Take(10)
                .Select(r => new ReviewResponse
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.UpdatedAt,
                    UserName = $"{r.User!.FirstName} {r.User!.LastName}",
                    UserEmail = r.User!.Email,
                    Reply = r.ReviewReply != null
                            ? new ReviewReplyResponse
                            {
                                Id = r.ReviewReply.Id,
                                Reply = r.ReviewReply.Reply ?? "",
                                CreatedAt = r.ReviewReply.UpdatedAt
                            }
                            : null
                })
            })
            .SingleOrDefaultAsync(cancellationToken);

        if(productResponse is not null)
        {
            Result<RatingBreakdownResponse>? ratingBreakdown = await reviewService
                .GetReviewRatingBreakdownAsync(productResponse.Id, cancellationToken);

            if(ratingBreakdown.IsSuccess)
                productResponse.RatingBreakdownResponse = ratingBreakdown.Data!;
        }

        return productResponse is null
            ? Result<ProductResponse>.Failure(Errors.ProductNotFound, HttpStatusCode.NotFound)
            : Result<ProductResponse>.Success(productResponse);
    }
}
