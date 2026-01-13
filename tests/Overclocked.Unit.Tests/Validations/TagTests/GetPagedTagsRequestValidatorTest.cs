using FluentValidation.TestHelper;
using Overclocked.Application.Features.TagUseCases.GetPagedTags;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.TagTests;

public class GetPagedTagsRequestValidatorTest
{
    [Fact]
    public void GetPagedTagsRequestValidator_Should_ReturnError_When_SortByIsInvalid()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var request = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "wrong",
            Direction = "Asc",
        };

        // Act
        TestValidationResult<GetPagedTagsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.SortBy).Only();
    }

    [Fact]
    public void GetPagedTagsRequestValidator_Should_ReturnError_When_DirectionIsInvalid()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var request = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "wrong",
        };

        // Act
        TestValidationResult<GetPagedTagsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Direction).Only();
    }

    [Fact]
    public void GetPagedTagsRequestValidator_Should_ReturnError_When_PageSizeIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var request = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 0,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "Asc",
        };

        // Act
        TestValidationResult<GetPagedTagsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }

    [Fact]
    public void GetPagedTagsRequestValidator_Should_ReturnError_When_PageIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var request = new GetPagedTagsRequest
        {
            Page = 0,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "Asc",
        };

        // Act
        TestValidationResult<GetPagedTagsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Page).Only();
    }

    [Fact]
    public void GetPagedTagsRequestValidator_Should_ReturnSuccess_When_AllParametersAreValid()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var request = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc",
        };

        // Act
        TestValidationResult<GetPagedTagsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }

    [Fact]
    public void GetPagedTagsRequestValidator_Should_ReturnError_When_AllAreWrongValues()
    {
        // Arrange
        var validator = new GetPagedTagsRequestValidator();
        var request = new GetPagedTagsRequest
        {
            Page = 0,
            PageSize = 0,
            SearchTerm = new string('a', 101),
            SortBy = "wrong",
            Direction = "wrong",
        };

        // Act
        TestValidationResult<GetPagedTagsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(5);
    }
}
