using FluentValidation.TestHelper;
using Overclocked.Application.Features.ProductUseCases.GetPagedProducts;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ProductTests;

public class GetPagedProductsRequestValidatorTest
{
    [Fact]
    public void GetPagedProductsRequestValidator_Should_ReturnError_When_SortByIsInvalid()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var request = new GetPagedProductsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "wrong",
            Direction = "Asc",
            BrandId = Guid.Empty,
            CategoryId = Guid.Empty,
            TagId = Guid.Empty,
        };

        // Act
        TestValidationResult<GetPagedProductsRequest> result = validator.TestValidate(request);

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
        var request = new GetPagedProductsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "wrong",
            BrandId = Guid.Empty,
            CategoryId = Guid.Empty,
            TagId = Guid.Empty,
        };

        // Act
        TestValidationResult<GetPagedProductsRequest> result = validator.TestValidate(request);

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
        var request = new GetPagedProductsRequest
        {
            Page = 1,
            PageSize = 0,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc",
            BrandId = Guid.Empty,
            CategoryId = Guid.Empty,
            TagId = Guid.Empty,
        };

        // Act
        TestValidationResult<GetPagedProductsRequest> result = validator.TestValidate(request);

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
        var request = new GetPagedProductsRequest
        {
            Page = 0,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc",
            BrandId = Guid.Empty,
            CategoryId = Guid.Empty,
            TagId = Guid.Empty,
        };

        // Act
        TestValidationResult<GetPagedProductsRequest> result = validator.TestValidate(request);

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
        var request = new GetPagedProductsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc",
            BrandId = Guid.Empty,
            CategoryId = Guid.Empty,
            TagId = Guid.Empty,
        };

        // Act
        TestValidationResult<GetPagedProductsRequest> result = validator.TestValidate(request);

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
        var request = new GetPagedProductsRequest
        {
            Page = 0,
            PageSize = 0,
            SearchTerm = new string('w', 110),
            SortBy = "wrong",
            Direction = "wrong",
            BrandId = Guid.Empty,
            CategoryId = Guid.Empty,
            TagId = Guid.Empty,
        };

        // Act
        TestValidationResult<GetPagedProductsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(5);
    }
}
