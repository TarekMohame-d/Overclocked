using Application.Features.Tag.Queries.GetPagedTags;
using Shouldly;

namespace Unit.Tests.Validations.Tag;

public class GetPagedTagsQueryValidationTest
{
    [Fact]
    public async Task TagValidator_WhenAllParametersAreValid_ShouldReturnSuccess()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidation();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "name_asc"
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count().ShouldBe(0);
    }

    [Fact]
    public async Task TagValidator_WhenPageSizeIsSmallerThan1_ShouldReturnError()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidation();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 0,
            SortBy = "name_asc"
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task TagValidator_WhenPageIsSmallerThan1_ShouldReturnError()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidation();
        var query = new GetPagedTagsQuery
        {
            Page = 0,
            PageSize = 10,
            SortBy = "name_asc"
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task ProductValidator_WhenFieldSortByIsNotValid_ShouldReturnError()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidation();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "wrong_asc"
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task ProductValidator_WhenDirectionSortByIsNotValid_ShouldReturnError()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidation();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SortBy = "name_wrong"
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
    }

    [Fact]
    public async Task TagValidator_WhenAllParametersAreInvalid_ShouldReturnError()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidation();
        var query = new GetPagedTagsQuery
        {
            Page = 0,
            PageSize = 0,
            SortBy = "wrong"
        };

        // Act
        var result = await validator.ValidateAsync(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(3);
    }
}
