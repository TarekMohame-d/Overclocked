using FluentValidation.TestHelper;
using Overclocked.Application.Features.ReviewUseCases.GetPagedReviews;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewTests;

public class GetPagedReviewsRequestValidatorTest
{
    [Fact]
    public void GetPagedReviewsRequestValidator_Should_ReturnError_When_SortByIsInvalid()
    {
        // Arrange
        var validator = new GetPagedReviewsRequestValidator();
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = "wrong",
            Direction = "Asc",
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<GetPagedReviewsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.SortBy).Only();
    }

    [Fact]
    public void GetPagedReviewsRequestValidator_Should_ReturnError_When_DirectionIsInvalid()
    {
        // Arrange
        var validator = new GetPagedReviewsRequestValidator();
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = "createdAt",
            Direction = "wrong",
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<GetPagedReviewsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Direction).Only();
    }

    [Fact]
    public void GetPagedReviewsRequestValidator_Should_ReturnError_When_PageSizeIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedReviewsRequestValidator();
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 0,
            SortBy = "createdAt",
            Direction = "Asc",
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<GetPagedReviewsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.PageSize).Only();
    }

    [Fact]
    public void GetPagedReviewsRequestValidator_Should_ReturnError_When_PageIsSmallerThanOne()
    {
        // Arrange
        var validator = new GetPagedReviewsRequestValidator();
        var request = new GetPagedReviewsRequest
        {
            Page = 0,
            PageSize = 10,
            SortBy = "createdAt",
            Direction = "Asc",
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<GetPagedReviewsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Page).Only();
    }

    [Fact]
    public void GetPagedReviewsRequestValidator_Should_ReturnSuccess_When_AllParametersAreValid()
    {
        // Arrange
        var validator = new GetPagedReviewsRequestValidator();
        var request = new GetPagedReviewsRequest
        {
            Page = 1,
            PageSize = 10,
            SortBy = "createdAt",
            Direction = "Asc",
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<GetPagedReviewsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }

    [Fact]
    public void GetPagedReviewsRequestValidator_Should_ReturnError_When_AllAreWrongValues()
    {
        // Arrange
        var validator = new GetPagedReviewsRequestValidator();
        var request = new GetPagedReviewsRequest
        {
            Page = 0,
            PageSize = 0,
            SortBy = "wrong",
            Direction = "wrong",
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<GetPagedReviewsRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(4);
    }
}
