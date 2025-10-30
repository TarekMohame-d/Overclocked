using Application.Common.Enums;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Validations;
using Shouldly;

namespace Unit.Tests.Validations.Tag;

public class GetPagedTagsRequestValidatorTest
{
    [Fact]
    public async Task GetPagedTagsRequestValidator_Should_ReturnSuccess_When_AllParametersAreValid()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var query = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = TagSortField.Name,
            Direction = Application.Common.Enums.SortDirection.Asc
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count().ShouldBe(0);
    }

    [Fact]
    public async Task GetPagedTagsRequestValidator_Should_ReturnError_When_PageSizeIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var query = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 0,
            SortBy = TagSortField.Name,
            Direction = Application.Common.Enums.SortDirection.Asc
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task GetPagedTagsRequestValidator_Should_ReturnError_When_PageIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var query = new GetPagedTagsRequest
        {
            Page = 0,
            PageSize = 10,
            SortBy = TagSortField.Name,
            Direction = Application.Common.Enums.SortDirection.Asc
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }
}
