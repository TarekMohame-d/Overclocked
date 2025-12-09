using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Product;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Unit.Tests.Validations.Product.TestCases;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Unit.Tests.Validations.Product;

public class CreateProductCommandValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();

        _validator = new CreateProductCommandValidator(
            _brandRepositoryMock,
            _categoryRepositoryMock,
            _productRepositoryMock,
            _tagRepositoryMock);
    }

    [Fact]
    public async Task CreateProductCommandValidator_Should_HaveError_When_BrandId_DoesNotExist()
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(specs: specs, tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns((BrandEntity)null!);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand> result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrandId).Only();
    }

    [Fact]
    public async Task CreateProductCommandValidator_Should_HaveError_When_CategoryId_DoesNotExist()
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(specs: specs, tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns((CategoryEntity)null!);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand> result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Name_Is_Invalid(string? name)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(specs: specs, tags: [tags.First().Id.Value], name: name!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Name_Is_Already_Exists()
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(specs: specs, tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidThumbnailCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Thumbnail_Is_Invalid(string? thumbnail)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            thumbnail: thumbnail!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thumbnail).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidDescriptionCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Description_Is_Invalid(string? description)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            description: description!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidPriceCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Price_Is_Invalid(decimal? price)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            price: (decimal)price!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidStockCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Stock_Is_Invalid(int? stock)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            stock: (int)stock!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StockQuantity).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidDiscountCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Discount_Is_Invalid(decimal? discount)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            discount: (decimal)discount!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidImagesCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Images_Is_Invalid(string[] images)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value],
            images: images);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Images").Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidTagsCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Tags_Is_Invalid(List<Guid> tags)
    {
        // Arrange
        IEnumerable<TagEntity> tagEntities = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        IEnumerable<(string Name, string Value)> specs = [new("Name", "Value")];
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: tags);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tagEntities);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Tags").Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateProductValidationTestCases.InvalidSpecificationsCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductCommandValidator_Should_HaveError_When_Specifications_Is_Invalid(
        IEnumerable<(string Name, string Value)> specs)
    {
        // Arrange
        IEnumerable<TagEntity> tags = new TagFaker().Generate(3);
        BrandEntity brand = new BrandFaker().Generate();
        CategoryEntity category = new CategoryFaker().Generate();
        CreateProductCommand command = CreateProductCommand(
            specs: specs,
            tags: [tags.First().Id.Value]);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<BrandId>())
            .Returns(brand);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<CategoryId>())
            .Returns(category);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock
            .WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(tags);

        // Act
        TestValidationResult<CreateProductCommand>? result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Specification").Only();
    }

    private static CreateProductCommand CreateProductCommand(
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
        return new CreateProductCommand
        {
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
