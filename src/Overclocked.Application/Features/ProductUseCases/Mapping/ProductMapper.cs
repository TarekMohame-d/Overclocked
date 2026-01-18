using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.Entities;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ProductUseCases.Mapping;

public static class ProductMapper
{
    public static Result<List<Specification>> CreateSpecifications(List<ProductSpecificationDto> specs)
    {
        List<Specification> specifications = [];

        foreach (ProductSpecificationDto spec in specs)
        {
            Result<Specification> specResult = Specification.Create(spec.Name, spec.Value);

            if (specResult.IsFailure)
                return Result.Failure<List<Specification>>(specResult.Error);

            specifications.Add(specResult.Value);
        }

        return Result.Success(specifications);
    }

    public static Result<List<ProductImage>> CreateProductImages(List<string>? images)
    {
        if (images is null || images.Count == 0)
            return Result.Success<List<ProductImage>>([]);

        List<ProductImage> productImages = [];

        foreach (var image in images)
        {
            Result<Image> imageResult = Image.Create(image);

            if (imageResult.IsFailure)
                return Result.Failure<List<ProductImage>>(imageResult.Error);

            productImages.Add(ProductImage.Create(imageResult.Value));
        }

        return Result.Success(productImages);
    }

    public static Result<List<ProductTag>> CreateProductTags(List<Guid> tags)
    {
        if (!tags.Any())
            return Result.Failure<List<ProductTag>>(ProductErrors.EmptyTags);

        if (tags.Distinct().Count() != tags.Count)
            return Result.Failure<List<ProductTag>>(ProductErrors.DuplicateTags);

        return Result.Success(tags.ConvertAll(t => ProductTag.Create(TagId.Create(t))));
    }
}
