using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using ArchitectureTests.FakeData;
using FluentValidation.TestHelper;
using NSubstitute;
using Unit.Tests.Validations.Product.TestCases;
using BrandEntity = Domain.Entities.Brand;
using CategoryEntity = Domain.Entities.Category;
using ProductEntity = Domain.Entities.Product;
using TagEntity = Domain.Entities.Tag;
using Application.Services.Product.Validations;
using Application.Services.Product.DTOs.Request;

namespace Unit.Tests.Validations.Product;

public class UpdateProductRequestValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock;
    private readonly ICategoryRepository _categoryRepositoryMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly UpdateProductRequestValidator _validator;

    public UpdateProductRequestValidatorTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();

        _validator = new UpdateProductRequestValidator(
            _brandRepositoryMock,
            _categoryRepositoryMock,
            _tagRepositoryMock);
    }

    [Fact]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_BrandId_DoesNotExist()
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns((BrandEntity)null!);

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BrandId).Only();
    }

    [Fact]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_CategoryId_DoesNotExist()
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns((CategoryEntity)null!);

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
            .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CategoryId).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Name_Is_Invalid(string? name)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), name: name!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidThumbnailCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Thumbnail_Is_Invalid(string? thumbnail)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), thumbnail: thumbnail!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Thumbnail).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidDescriptionCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Description_Is_Invalid(string? description)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), description: description!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidPriceCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Price_Is_Invalid(decimal? price)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), price: (decimal)price!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidStockCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Stock_Is_Invalid(int? stock)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), stock: (int)stock!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Stock).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidDiscountCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Discount_Is_Invalid(decimal? discount)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), discount: (decimal)discount!);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidImagesCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Images_Is_Invalid(string[] images)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id), images: images);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Images).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidTagsCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Tags_Is_Invalid(IEnumerable<Guid> tags)
    {
        // Arrange
        var tagEntities = new TagFaker().Generate(3);
        IEnumerable<UpdateProductRequest.Specs> specs = [new UpdateProductRequest.Specs { Name = "Name", Value = "Value" }];
        var request = UpdateProductRequest(specs, tags);

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tagEntities));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Tags).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateProductValidationTestCases.InvalidSpecificationsCases), MemberType = typeof(UpdateProductValidationTestCases))]
    public async Task UpdateProductRequestValidator_Should_HaveError_When_Specifications_Is_Invalid(IEnumerable<UpdateProductRequest.Specs> specs)
    {
        // Arrange
        var tags = new TagFaker().Generate(3);
        var request = UpdateProductRequest(specs, tags.Select(x => x.Id));

        _brandRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<BrandEntity>());

        _categoryRepositoryMock.GetByIdAsync(Arg.Any<object[]>())
            .Returns(Substitute.For<CategoryEntity>());

        _tagRepositoryMock.WhereAsync(Arg.Any<Expression<Func<TagEntity, bool>>>())
        .Returns(Task.FromResult<IEnumerable<TagEntity>>(tags));

        // Act
        var result = await _validator.TestValidateAsync(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Specification).Only();
    }

    private UpdateProductRequest UpdateProductRequest(
        IEnumerable<UpdateProductRequest.Specs> specs,
        IEnumerable<Guid> tags,
        IEnumerable<string>? images = null,
        Guid? id = null,
        Guid? brandId = null,
        Guid? categoryId = null,
        string name = "Product Name",
        string description = "Product Description",
        string thumbnail = "https://res.cloudinary.com/over-clocked/image.png",
        decimal price = 100,
        decimal discount = 0m,
        int stock = 10)
    {
        return new UpdateProductRequest
        {
            Id = id ?? Guid.CreateVersion7(),
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
