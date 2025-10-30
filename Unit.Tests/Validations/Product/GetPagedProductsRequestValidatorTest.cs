using Application.Common.Enums;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.Validations;
using Shouldly;

namespace Unit.Tests.Validations.Product;

public class GetPagedProductsRequestValidatorTest
{
    [Fact]
    public async Task GetPagedProductsRequestValidator_Should_ReturnSuccess_WhenWhen_AllParametersAreValid()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var request = new GetPagedProductsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = ProductSortField.Name,
            Direction = Application.Common.Enums.SortDirection.Asc
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetPagedProductsRequestValidator_Should_ReturnError_When_PageSizeIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var request = new GetPagedProductsRequest
        {
            Page = 1,
            PageSize = 0,
            SortBy = ProductSortField.Name,
            Direction = Application.Common.Enums.SortDirection.Asc
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task GetPagedProductsRequestValidator_Should_ReturnError_When_PageIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedProductsRequestValidator();
        var request = new GetPagedProductsRequest
        {
            Page = 0,
            PageSize = 10,
            SortBy = ProductSortField.Name,
            Direction = Application.Common.Enums.SortDirection.Asc
        };

        // Act
        var result = await validator.ValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }
}
