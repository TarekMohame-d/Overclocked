using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Validations;
using FluentValidation.TestHelper;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Brand.TestCases;
using BrandEntity = Domain.Entities.Brand;

namespace Unit.Tests.Validations.Brand;

public class CreateBrandRequestValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock = Substitute.For<IBrandRepository>();

    [Theory]
    [MemberData(
        nameof(CreateBrandValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateBrandValidationTestCases)
    )]
    public async Task CreateBrandRequestValidator_Should_ReturnError_When_NameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateBrandRequestValidator(_brandRepositoryMock);
        var request = new CreateBrandRequest
        {
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateBrandValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(CreateBrandValidationTestCases)
    )]
    public async Task CreateBrandRequestValidator_Should_ReturnError_When_ImageIsInvalid(string? imageUrl)
    {
        // Arrange
        var validator = new CreateBrandRequestValidator(_brandRepositoryMock);
        var request = new CreateBrandRequest { Name = "Brand Name", ImageUrl = imageUrl! };

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task CreateBrandRequestValidator_Should_ReturnError_When_NameAlreadyExists()
    {
        // Arrange
        var validator = new CreateBrandRequestValidator(_brandRepositoryMock);
        var request = new CreateBrandRequest
        {
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
