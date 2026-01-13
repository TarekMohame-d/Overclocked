using FluentValidation.TestHelper;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.UpdateProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.TagAggregate;
using Overclocked.Unit.Tests.Validations.ProductTests.TestCases;

namespace Overclocked.Unit.Tests.Validations.ProductTests;

public class UpdateProductRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Name_Is_Invalid(string? name)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], name: name!);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidThumbnailCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Thumbnail_Is_Invalid(string? thumbnail)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], thumbnail: thumbnail!);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thumbnail).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidDescriptionCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Description_Is_Invalid(string? description)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], description: description!);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidPriceCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Price_Is_Invalid(decimal? price)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], price: (decimal)price!);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidStockCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Stock_Is_Invalid(int? stock)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], stock: (int)stock!);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidDiscountCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Discount_Is_Invalid(decimal? discount)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], discount: (decimal)discount!);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidImagesCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Images_Is_Invalid(string[] images)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value], images: images.ToList());

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Images).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidTagsCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Tags_Is_Invalid(List<Guid> tags)
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tagEntities = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        List<ProductSpecificationDto> specs = [new ProductSpecificationDto { Name = "Name", Value = "Value" }];

        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: tags);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidSpecificationsCases),
        MemberType = typeof(UpdateProductValidationTestCases)
    )]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Specifications_Is_Invalid(
        List<ProductSpecificationDto> specs
    )
    {
        // Arrange
        var validator = new UpdateProductRequestValidator();

        List<Tag> tags = new TagFaker().Generate(3);
        Brand brand = new BrandFaker().Generate();
        Category category = new CategoryFaker().Generate();
        UpdateProductRequest request = UpdateProductRequest(specs: specs, tags: [tags[0].Id.Value]);

        // Act
        TestValidationResult<UpdateProductRequest>? result = await validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specifications).Only();
    }

    private static UpdateProductRequest UpdateProductRequest(
        List<ProductSpecificationDto>? specs = null,
        List<Guid>? tags = null,
        List<string>? images = null,
        Guid? brandId = null,
        Guid? categoryId = null,
        string name = "Product Name",
        string description = "Product Description",
        string thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
        decimal price = 100,
        decimal discount = 0m,
        int stock = 10
    ) =>
        new()
        {
            Id = Guid.NewGuid(),
            BrandId = brandId ?? Guid.NewGuid(),
            CategoryId = categoryId ?? Guid.NewGuid(),
            Name = name,
            Thumbnail = thumbnail,
            Description = description,
            Price = price,
            StockQuantity = stock,
            Discount = discount,
            Tags = tags ?? [],
            Images = images ?? [],
            Specifications = specs ?? [],
        };
}
