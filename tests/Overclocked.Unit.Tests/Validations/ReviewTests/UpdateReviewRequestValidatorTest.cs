using FluentValidation.TestHelper;
using Overclocked.Application.Features.ReviewUseCases.UpdateReview;
using Overclocked.Unit.Tests.Validations.ReviewTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewTests;

public class UpdateReviewRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateReviewValidationTestCases.InvalidRatingCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task UpdateReviewRequestValidator_Should_ReturnError_WhenRatingIsInvalid(int? rating)
    {
        // Arrange
        var validator = new UpdateReviewRequestValidator();

        var request = new UpdateReviewRequest
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = (int)rating!,
            Comment = "Comment",
        };

        // Act
        TestValidationResult<UpdateReviewRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Rating).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateReviewValidationTestCases.InvalidCommentCases),
        MemberType = typeof(UpdateReviewValidationTestCases)
    )]
    public async Task UpdateReviewRequestValidator_Should_ReturnError_WhenCommentIsInvalid(string? comment)
    {
        // Arrange
        var validator = new UpdateReviewRequestValidator();

        var request = new UpdateReviewRequest
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            Rating = 3,
            Comment = comment!,
        };

        // Act
        TestValidationResult<UpdateReviewRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Comment).Only();
    }
}
