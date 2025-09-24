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

    [Theory]
    [MemberData(nameof(CreateBrandValidationTestCases.InvalidNameCases), MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task BrandValidator_WhenNameIsInvalid_ShouldReturnError(string? name)
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand
        {
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
        if (!string.IsNullOrWhiteSpace(name))
            await _brandRepositoryMock.Received(1)
                .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(nameof(CreateBrandValidationTestCases.InvalidImageUrlCases), MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task BrandValidator_WhenImageIsInvalid_ShouldReturnError(string? imageUrl)
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand
        {
            Name = "Nike",
            ImageUrl = imageUrl!
        };

        _brandRepositoryMock.AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
        await _brandRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task BrandValidator_WhenNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand
        {
            Name = "Nike",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

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
