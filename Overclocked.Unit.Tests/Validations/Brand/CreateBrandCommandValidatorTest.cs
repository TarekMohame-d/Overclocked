using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Brand.Commands.CreateBrand;
using Overclocked.Unit.Tests.Validations.Brand.TestCases;
using Shouldly;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;

namespace Overclocked.Unit.Tests.Validations.Brand;

public class CreateBrandCommandValidatorTest
{
    private readonly IBrandRepository _brandRepositoryMock = Substitute.For<IBrandRepository>();

    [Theory]
    [MemberData(
        nameof(CreateBrandValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task CreateBrandCommandValidator_Should_ReturnError_When_NameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand(name!, "https://res.cloudinary.com/over-clocked/image.png");

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateBrandValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task CreateBrandCommandValidator_Should_ReturnError_When_ImageIsInvalid(string? imageUrl)
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand("Brand Name", imageUrl!);

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task CreateBrandCommandValidator_Should_ReturnError_When_NameAlreadyExists()
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand("Brand Name", "https://res.cloudinary.com/over-clocked/image.png");

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task CreateBrandCommandValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand(string.Empty, "imageUrl");

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task CreateBrandCommandValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new CreateBrandCommandValidator(_brandRepositoryMock);
        var command = new CreateBrandCommand("Brand Name", "https://res.cloudinary.com/over-clocked/image.png");

        _brandRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<BrandEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
