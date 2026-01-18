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

namespace Overclocked.Application.Features.ProductUseCases.CreateProduct;

public class CreateProductRequestHandler(
    IProductRepository productRepository,
    IBrandRepository brandRepository,
    ICategoryRepository categoryRepository,
    ITagReadRepository tagRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateProductRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateProductRequest request, CancellationToken ct)
    {
        if (await productRepository.NameExistsAsync(request.Name, ct))
            return Result.Failure<Guid>(ProductErrors.ProductNameAlreadyExists);

        if (!await brandRepository.ExistsAsync(BrandId.Create(request.BrandId), ct))
            return Result.Failure<Guid>(BrandErrors.BrandNotFound(request.BrandId));

        if (!await categoryRepository.ExistsAsync(CategoryId.Create(request.CategoryId), ct))
            return Result.Failure<Guid>(CategoryErrors.CategoryNotFound(request.CategoryId));

        List<Guid> existingIds = await tagRepository.GetExistingTagIdsAsync(request.Tags, ct);

        var missingTags = request.Tags.Except(existingIds).ToList();

        if (missingTags.Count > 0)
        {
            return Result.Failure<Guid>(TagErrors.TagsNotFound(missingTags));
        }

        Result<Image> thumbnailResult = Image.Create(request.Thumbnail.Trim());
        if (thumbnailResult.IsFailure)
            return Result.Failure<Guid>(thumbnailResult.Error);

        Result<Money> priceResult = Money.Create(request.Price);
        if (priceResult.IsFailure)
            return Result.Failure<Guid>(priceResult.Error);

        Result<DiscountRate> discountResult = DiscountRate.Create(request.Discount ?? 0.0m);
        if (discountResult.IsFailure)
            return Result.Failure<Guid>(discountResult.Error);

        Result<List<Specification>> specificationsResult = ProductMapper.CreateSpecifications(request.Specifications);
        if (specificationsResult.IsFailure)
            return Result.Failure<Guid>(specificationsResult.Error);

        Result<List<ProductImage>> imagesResult = ProductMapper.CreateProductImages(request.Images);
        if (imagesResult.IsFailure)
            return Result.Failure<Guid>(imagesResult.Error);

        Result<List<ProductTag>> tagsResult = ProductMapper.CreateProductTags(request.Tags);
        if (tagsResult.IsFailure)
            return Result.Failure<Guid>(tagsResult.Error);

        Result<Product> productResult = Product.Create(
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
            tagsResult.Value
        );

        if (productResult.IsFailure)
            return Result.Failure<Guid>(productResult.Error);

        Product product = productResult.Value;

        productRepository.Add(product);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(product.Id.Value);
    }
}
