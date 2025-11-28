using Application.Services.Review.DTOs.Request;
using Application.Services.Review.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Review.TestCases;

namespace Unit.Tests.Validations.Review;

public class CreateReviewRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(CreateReviewValidationTestCases.InvalidRatingCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task CreateReviewRequestValidator_Should_ReturnError_WhenRatingIsInvalid(int? rating)
    {
        // Arrange
        var validator = new CreateReviewRequestValidator();

        var request = new CreateReviewRequestBody
        {
            Rating = (int)rating!,
            Comment = "Comment"
        };

        // Act
        TestValidationResult<CreateReviewRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Rating).Only();
    }

    [Theory]
    [MemberData(nameof(CreateReviewValidationTestCases.InvalidCommentCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task CreateReviewRequestValidator_Should_ReturnError_WhenCommentIsInvalid(string? comment)
    {
        // Arrange
        var validator = new CreateReviewRequestValidator();

        var request = new CreateReviewRequestBody
        {
            Rating = 2,
            Comment = comment!
        };

        // Act
        TestValidationResult<CreateReviewRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Comment).Only();
    }
}
