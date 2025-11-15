using Application.Services.Product.DTOs.Request;
using Application.Services.Product.Validations;
using FluentValidation.TestHelper;
using Shouldly;

namespace Unit.Tests.Validations.Product;

public class GetPagedProductsRequestValidatorTest
{
    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnError_When_SortByIsInvalid()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var query = new GetPagedProductsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "wrong",
            Direction = "Asc"
        };

        // Act
        TestValidationResult<GetPagedProductsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.SortBy).Only();
    }

    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnError_When_DirectionIsInvalid()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var query = new GetPagedProductsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "id",
            Direction = "wrong"
        };

        // Act
        TestValidationResult<GetPagedProductsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Direction).Only();
    }

    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnError_When_PageSizeIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var query = new GetPagedProductsQuery
        {
            Page = 1,
            PageSize = 0,
            SortBy = "id",
            Direction = "Asc"
        };

        // Act
        TestValidationResult<GetPagedProductsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }

    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnError_When_PageIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var query = new GetPagedProductsQuery
        {
            Page = 0,
            PageSize = 10,
            SortBy = "id",
            Direction = "Asc"
        };

        // Act
        TestValidationResult<GetPagedProductsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Page).Only();
    }

    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnSuccess_When_AllParametersAreValid()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var query = new GetPagedProductsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "id",
            Direction = "Asc"
        };

        // Act
        TestValidationResult<GetPagedProductsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }

    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnError_When_AllAreWrongValues()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var query = new GetPagedProductsQuery
        {
            Page = 0,
            PageSize = 0,
            SortBy = "wrong",
            Direction = "wrong"
        };

        // Act
        TestValidationResult<GetPagedProductsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(4);
    }
}
