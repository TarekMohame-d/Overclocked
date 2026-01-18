using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ProductUseCases.Mapping;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.Entities;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ProductUseCases.UpdateProduct;

public class UpdateProductRequestHandler(
    IProductRepository productRepository,
    IBrandRepository brandRepository,
    ICategoryRepository categoryRepository,
    ITagReadRepository tagRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateProductRequest>
{
    public async Task<Result> Handle(UpdateProductRequest request, CancellationToken ct)
    {
        Product? product = await productRepository.GetByIdAsync(ProductId.Create(request.Id), ct);

        if (product is null)
            return Result.Failure(ProductErrors.ProductNotFound(request.Id));

        if (!await brandRepository.ExistsAsync(BrandId.Create(request.BrandId), ct))
            return Result.Failure<Guid>(BrandErrors.BrandNotFound(request.BrandId));

        if (!await categoryRepository.ExistsAsync(CategoryId.Create(request.CategoryId), ct))
            return Result.Failure<Guid>(CategoryErrors.CategoryNotFound(request.CategoryId));

        List<Guid> existingIds = await tagRepository.GetExistingTagIdsAsync(request.Tags, ct);

        var missingTags = request.Tags.Except(existingIds).ToList();

        if (missingTags.Count > 0)
        {
            return Result.Failure(TagErrors.TagsNotFound(missingTags));
        }

        Result<Image> thumbnailResult = Image.Create(request.Thumbnail.Trim());
        if (thumbnailResult.IsFailure)
            return Result.Failure(thumbnailResult.Error);

        Result<Money> priceResult = Money.Create(request.Price);
        if (priceResult.IsFailure)
            return Result.Failure(priceResult.Error);

        Result<DiscountRate> discountResult = DiscountRate.Create(request.Discount ?? 0.0m);
        if (discountResult.IsFailure)
            return Result.Failure(discountResult.Error);

        Result<List<Specification>> specificationsResult = ProductMapper.CreateSpecifications(request.Specifications);
        if (specificationsResult.IsFailure)
            return Result.Failure(specificationsResult.Error);

        Result<List<ProductImage>> imagesResult = ProductMapper.CreateProductImages(request.Images);
        if (imagesResult.IsFailure)
            return Result.Failure(imagesResult.Error);

        Result<List<ProductTag>> tagsResult = ProductMapper.CreateProductTags(request.Tags);
        if (tagsResult.IsFailure)
            return Result.Failure(tagsResult.Error);

        if (product.Name != request.Name)
        {
            var exist = await productRepository.NameExistsAsync(request.Name, ct);

            if (exist)
                return Result.Failure(ProductErrors.ProductNameAlreadyExists);
        }

        Result result = product.Update(
            BrandId.Create(request.BrandId),
            CategoryId.Create(request.CategoryId),
            request.Name,
            request.Description,
            thumbnailResult.Value,
            request.StockQuantity,
            priceResult.Value,
            discountResult.Value,
            imagesResult.Value,
            specificationsResult.Value,
            tagsResult.Value,
            request.IsDeleted
        );

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
