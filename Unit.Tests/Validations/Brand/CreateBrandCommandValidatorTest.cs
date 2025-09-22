using System.Linq.Expressions;
using Application.Features.Brand.Commands.CreateBrand;
using Domain.Repositories;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Brand.TestCases;
using BrandEntity = Domain.Entities.Brand;

namespace Unit.Tests.Validations.Brand;

public class CreateBrandCommandValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock;

    public CreateBrandCommandValidatorTest()
    {
        _brandRepositoryMock = Substitute.For<IBrandRepository>();
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
    [MemberData(nameof(CreateBrandValidationTestCases.InvalidNameCases), MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task BrandValidator_WhenNameIsInvalid_ShouldReturnError(string? name)
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand { Name = name!, ImageFile = CreateImageFile() };

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(CreateBrandValidationTestCases.InvalidImageFileCases), MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task BrandValidator_WhenImageIsInvalid_ShouldReturnError(IFormFile? image)
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand { Name = "Nike", ImageFile = image! };

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageFile").ShouldBeTrue();
        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BrandValidator_WhenNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand { Name = "Nike", ImageFile = CreateImageFile() };

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();

        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
