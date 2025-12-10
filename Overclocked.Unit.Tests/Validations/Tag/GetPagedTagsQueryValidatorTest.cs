using FluentValidation.TestHelper;
using Overclocked.Application.Tag.Queries.GetTags;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Tag;

public class GetPagedTagsQueryValidatorTest
{
    [Fact]
    public void GetPagedTagsQueryValidator_Should_ReturnError_When_SortByIsInvalid()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidator();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "wrong",
            Direction = "Asc",
        };

        // Act
        TestValidationResult<GetPagedTagsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.SortBy).Only();
    }

    [Fact]
    public void GetPagedTagsQueryValidator_Should_ReturnError_When_DirectionIsInvalid()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidator();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "wrong",
        };

        // Act
        TestValidationResult<GetPagedTagsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Direction).Only();
    }

    [Fact]
    public void GetPagedTagsQueryValidator_Should_ReturnError_When_PageSizeIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidator();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 0,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "Asc",
        };

        // Act
        TestValidationResult<GetPagedTagsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }

    [Fact]
    public void GetPagedTagsQueryValidator_Should_ReturnError_When_PageIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidator();
        var query = new GetPagedTagsQuery
        {
            Page = 0,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "Asc",
        };

        // Act
        TestValidationResult<GetPagedTagsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Page).Only();
    }

    [Fact]
    public void GetPagedTagsQueryValidator_Should_ReturnSuccess_When_AllParametersAreValid()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidator();
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc",
        };

        // Act
        TestValidationResult<GetPagedTagsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }

    [Fact]
    public void GetPagedTagsQueryValidator_Should_ReturnError_When_AllAreWrongValues()
    {
        // Arrange
        var validator = new GetPagedTagsQueryValidator();
        var query = new GetPagedTagsQuery
        {
            Page = 0,
            PageSize = 0,
            SearchTerm = string.Empty,
            SortBy = "wrong",
            Direction = "wrong",
        };

        // Act
        TestValidationResult<GetPagedTagsQuery> result = validator.TestValidate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(5);
    }
}
