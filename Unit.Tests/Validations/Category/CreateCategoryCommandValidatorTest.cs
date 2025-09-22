using System.Linq.Expressions;
using Application.Features.Category.Commands.CreateCategory;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
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

    private static IFormFile CreateImageFile(string fileName = "test.jpg", long size = 1024, string contentType = "image/jpeg")
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.Length.Returns(size);
        file.ContentType.Returns(contentType);
        file.OpenReadStream().Returns(new MemoryStream(new byte[size]));
        return file;
    }

    [Theory]
    [MemberData(nameof(CreateCategoryValidationTestCases.InvalidNameCases), MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CategoryValidator_WhenNameIsInvalid_ShouldReturnError(string? name)
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand { Name = name!, ImageFile = CreateImageFile() };

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
    [MemberData(nameof(CreateCategoryValidationTestCases.InvalidImageFileCases), MemberType = typeof(CreateCategoryValidationTestCases))]
    public async Task CategoryValidator_WhenImageIsInvalid_ShouldReturnError(IFormFile? image)
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand { Name = "Nike", ImageFile = image! };

        _categoryRepositoryMock.AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageFile").ShouldBeTrue();
        await _categoryRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<CategoryEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CategoryValidator_WhenNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var validator = new CreateCategoryCommandValidator(_categoryRepositoryMock);
        var command = new CreateCategoryCommand { Name = "Nike", ImageFile = CreateImageFile() };

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
