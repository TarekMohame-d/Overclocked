using Application.Services.Review.DTOs.Request;
using Application.Services.Review.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Review.TestCases;

namespace Unit.Tests.Validations.Review;

public class UpdateReviewRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateReviewValidationTestCases.InvalidRatingCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task UpdateReviewRequestValidator_Should_ReturnError_WhenRatingIsInvalid(int? rating)
    {
        // Arrange
        var validator = new UpdateReviewRequestValidator();

        var request = new UpdateReviewRequestBody
        {
            Rating = (int)rating!,
            Comment = "Comment"
        };

        // Act
        TestValidationResult<UpdateReviewRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Rating).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateReviewValidationTestCases.InvalidCommentCases), MemberType = typeof(UpdateReviewValidationTestCases))]
    public async Task UpdateReviewRequestValidator_Should_ReturnError_WhenCommentIsInvalid(string? comment)
    {
        // Arrange
        var validator = new UpdateReviewRequestValidator();

        var request = new UpdateReviewRequestBody
        {
            Rating = 2,
            Comment = comment!
        };

        // Act
        TestValidationResult<UpdateReviewRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Comment).Only();
    }
}
