using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.Validations;
using ArchitectureTests.FakeData;
using FluentValidation.TestHelper;
using NSubstitute;
using Unit.Tests.Validations.Product.TestCases;
using BrandEntity = Domain.Entities.Brand;
using CategoryEntity = Domain.Entities.Category;
using ProductEntity = Domain.Entities.Product;
using TagEntity = Domain.Entities.Tag;

namespace Unit.Tests.Validations.Product;

public class CreateProductRequestValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly CreateProductRequestValidator _validator;

    public CreateProductRequestValidatorTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();

        _validator = new CreateProductRequestValidator(
            _brandRepositoryMock,
            _categoryRepositoryMock,
            _productRepositoryMock,
            _tagRepositoryMock);
    }

    [Fact]
    public async Task CreateProductRequestValidator_Should_HaveError_When_BrandId_DoesNotExist()
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns((BrandEntity)null!);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrandId).Only();
    }

    [Fact]
    public async Task CreateProductRequestValidator_Should_HaveError_When_CategoryId_DoesNotExist()
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns((CategoryEntity)null!);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Name_Is_Invalid(string? name)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id), name: name!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Name_Is_Already_Exists()
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidThumbnailCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Thumbnail_Is_Invalid(string? thumbnail)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id), thumbnail: thumbnail!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thumbnail).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidDescriptionCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Description_Is_Invalid(string? description)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id), description: description!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidPriceCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Price_Is_Invalid(decimal? price)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id), price: (decimal)price!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidStockCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Stock_Is_Invalid(int? stock)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id), stock: (int)stock!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidDiscountCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Discount_Is_Invalid(decimal? discount)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request =
            CreateProductRequest(specs, tags.Select(x => x.Id), discount: (decimal)discount!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidImagesCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Images_Is_Invalid(string[] images)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id), images);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Images).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidTagsCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Tags_Is_Invalid(List<Guid> tags)
    {
        // Arrange
        List<TagEntity>? tagEntities = new TagFaker().Generate(3);
        IEnumerable<CreateProductRequest.Specs> specs =
            [new() { Name = "Name", Value = "Value" }];
        CreateProductRequest request = CreateProductRequest(specs, tags);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tagEntities));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidSpecificationsCases),
        MemberType = typeof(CreateProductValidationTestCases))]
    public async Task CreateProductRequestValidator_Should_HaveError_When_Specifications_Is_Invalid(
        IEnumerable<CreateProductRequest.Specs> specs)
    {
        // Arrange
        List<TagEntity>? tags = new TagFaker().Generate(3);
        CreateProductRequest request = CreateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        TestValidationResult<CreateProductRequest>? result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specification).Only();
    }

    private static CreateProductRequest CreateProductRequest(
        IEnumerable<CreateProductRequest.Specs> specs,
        IEnumerable<Guid> tags,
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
        return new CreateProductRequest
        {
            BrandId = brandId ?? Guid.CreateVersion7(),
            CategoryId = categoryId ?? Guid.CreateVersion7(),
            Name = name,
            Description = description,
            Thumbnail = thumbnail,
            Price = price,
            Discount = discount,
            Stock = stock,
            Specification = specs,
            Tags = tags,
            Images = images
        };
    }
}
