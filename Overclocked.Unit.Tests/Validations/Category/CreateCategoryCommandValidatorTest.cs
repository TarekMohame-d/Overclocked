using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Category.Commands.CreateCategory;
using Overclocked.Unit.Tests.Validations.Category.TestCases;
using Shouldly;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;

namespace Overclocked.Unit.Tests.Validations.Category;

public class CreateCategoryCommandValidatorTest
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

    [Theory]
    [MemberData(
        nameof(CreateCategoryValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CreateCategoryCommandValidator_Should_ReturnError_When_NameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateCategoryValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CreateCategoryCommandValidator_Should_ReturnError_When_ImageIsInvalid(string? imageUrl)
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = "Category Name",
            ImageUrl = imageUrl!
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_Should_ReturnError_When_NameAlreadyExists()
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = string.Empty,
            ImageUrl = "imageUrl"
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task CreateCategoryCommandValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
