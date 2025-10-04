using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Features.Category.Commands.CreateCategory;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Category.TestCases;
using CategoryEntity = Domain.Entities.Category;

namespace Unit.Tests.Validations.Category;

public class CreateCategoryCommandValidatorTest
{
    private readonly ICategoryRepository _categoryRepositoryMock;

    public CreateCategoryCommandValidatorTest()
    {
        _categoryRepositoryMock = Substitute.For<ICategoryRepository>();
    }

    [Theory]
    [MemberData(nameof(CreateCategoryValidationTestCases.InvalidNameCases), MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CategoryValidator_WhenNameIsInvalid_ShouldReturnError(string? name)
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
        if (!string.IsNullOrWhiteSpace(name))
            await _categoryRepositoryMock.Received(1)
                .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(CreateCategoryValidationTestCases.InvalidImageUrlCases), MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CategoryValidator_WhenImageIsInvalid_ShouldReturnError(string? imageUrl)
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = "Nike",
            ImageUrl = imageUrl!
        };

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CategoryValidator_WhenNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = "Nike",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();

        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
