using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Validations;
using FluentValidation.TestHelper;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Category.TestCases;
using CategoryEntity = Domain.Entities.Category;

namespace Unit.Tests.Validations.Category;

public class CreateCategoryRequestValidatorTest
{
    private readonly ICategoryRepository _categoryRepositoryMock = Substitute.For<ICategoryRepository>();

    [Theory]
    [MemberData(nameof(CreateCategoryValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CreateCategoryRequestValidator_WhenNameIsInvalid_ShouldReturnError(string? name)
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator(_categoryRepositoryMock);
        var request = new CreateCategoryRequest
        {
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(CreateCategoryValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CreateCategoryRequestValidator_WhenImageIsInvalid_ShouldReturnError(string? imageUrl)
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator(_categoryRepositoryMock);
        var request = new CreateCategoryRequest
        {
            Name = "Category Name",
            ImageUrl = imageUrl!
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task CreateCategoryRequestValidator_WhenNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator(_categoryRepositoryMock);
        var request = new CreateCategoryRequest
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _categoryRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
