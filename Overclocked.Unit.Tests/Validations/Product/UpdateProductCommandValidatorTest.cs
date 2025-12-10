using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Unit.Tests.Validations.Product.TestCases;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Unit.Tests.Validations.Product;

public class UpdateProductCommandValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly UpdateProductCommandValidator _validator;

    public UpdateProductCommandValidatorTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();

        _validator = new UpdateProductCommandValidator(
            _brandRepositoryMock,
            _categoryRepositoryMock,
            _tagRepositoryMock);
    }

    [Fact]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_BrandId_DoesNotExist()
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(specs: specs, tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns((BrandEntity)null!);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand> result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrandId).Only();
    }

    [Fact]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_CategoryId_DoesNotExist()
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(specs: specs, tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns((CategoryEntity)null!);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand> result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Name_Is_Invalid(string? name)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(specs: specs, tags: [tags.First().Id.Value], name: name!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidThumbnailCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Thumbnail_Is_Invalid(string? thumbnail)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            thumbnail: thumbnail!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thumbnail).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidDescriptionCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Description_Is_Invalid(string? description)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            description: description!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidPriceCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Price_Is_Invalid(decimal? price)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            price: (decimal)price!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidStockCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Stock_Is_Invalid(int? stock)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            stock: (int)stock!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidDiscountCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Discount_Is_Invalid(decimal? discount)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            discount: (decimal)discount!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidImagesCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Images_Is_Invalid(string[] images)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            images: images);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Images").Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidTagsCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Tags_Is_Invalid(List<Guid> tags)
    {
        // Arrange
        IEnumerable<TagEntity> tagEntities = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: tags);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tagEntities);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags").Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateProductValidationTestCases.InvalidSpecificationsCases),
        MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductCommandValidator_Should_HaveError_When_Specifications_Is_Invalid(
        IEnumerable<(string Name, string Value)> specs)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        UpdateProductCommand command = UpdateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<UpdateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Specification").Only();
    }

    private static UpdateProductCommand UpdateProductCommand(
        IEnumerable<(string Name, string Value)>? specs = null,
        IEnumerable<Guid>? tags = null,
        IEnumerable<string>? images = null,
        Guid? brandId = null,
        Guid? categoryId = null,
        string name = "Product Name",
        string description = "Product Description",
        string thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
        decimal price = 100,
        decimal discount = 0m,
        int stock = 10)
    {
        return new UpdateProductCommand
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
            Specifications = specs ?? []
        };
    }
}
