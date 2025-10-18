using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Features.Product.Commands.CreateProduct;
using ArchitectureTests.FakeData;
using FluentValidation.TestHelper;
using NSubstitute;
using Unit.Tests.Validations.Product.TestCases;
using BrandEntity = Domain.Entities.Brand;
using CategoryEntity = Domain.Entities.Category;
using ProductEntity = Domain.Entities.Product;
using TagEntity = Domain.Entities.Tag;

namespace Unit.Tests.Validations.Product;

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
    public async Task Should_HaveError_When_BrandId_DoesNotExist()
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns((BrandEntity)null!);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrandId).Only();
    }

    [Fact]
    public async Task Should_HaveError_When_CategoryId_DoesNotExist()
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns((CategoryEntity)null!);

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidNameCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Name_Is_Invalid(string? name)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = name!,
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task Should_HaveError_When_Name_Is_Already_Exists()
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidThumbnailCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Thumbnail_Is_Invalid(string? thumbnail)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = thumbnail!,
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thumbnail).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidDescriptionCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Description_Is_Invalid(string? description)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = description!,
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidPriceCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Price_Is_Invalid(decimal? price)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = (decimal)price!,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidStockCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Stock_Is_Invalid(int? stock)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Stock = (int)stock!,
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id]
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidDiscountCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Discount_Is_Invalid(decimal? discount)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id],
            Discount = (decimal)discount!
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidImagesCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Images_Is_Invalid(string[] images)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = [tag.Id],
            Images = images
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Images).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidTagsCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Tags_Is_Invalid(List<Guid> tags)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = [new Specs { Name = "Name", Value = "Value" }],
            Tags = tags
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags).Only();
    }

    [Theory]
    [MemberData(nameof(CreateProductValidationTestCases.InvalidSpecificationsCases), MemberType = typeof(CreateProductValidationTestCases))]
    public async Task Should_HaveError_When_Specifications_Is_Invalid(List<Specs> specs)
    {
        // Arrange
        var tag = new TagFaker().Generate();
        var command = new CreateProductCommand
        {
            BrandId = Guid.NewGuid(),
            CategoryId = Guid.NewGuid(),
            Name = "Product Name",
            Description = "Product Description",
            Thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
            Price = 100,
            Specification = specs,
            Tags = [tag.Id],
        };
        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _productRepositoryMock.AnyAsync(Arg.Any<Expression<Func<ProductEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>([tag]));

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specification).Only();
    }
}
