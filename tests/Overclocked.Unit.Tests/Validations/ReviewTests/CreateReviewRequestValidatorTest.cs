using FluentValidation.TestHelper;
using Overclocked.Application.Features.ReviewUseCases.CreateReview;
using Overclocked.Unit.Tests.Validations.ReviewTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewTests;

public class CreateReviewRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(CreateReviewValidationTestCases.InvalidRatingCases), MemberType = typeof(CreateReviewValidationTestCases))]
    public async Task CreateReviewRequestValidator_Should_ReturnError_WhenRatingIsInvalid(int? rating)
    {
        // Arrange
        var validator = new CreateReviewRequestValidator();

        var request = new CreateReviewRequest
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = (int)rating!,
            Comment = "Comment",
        };

        // Act
        TestValidationResult<CreateReviewRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Rating).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateReviewValidationTestCases.InvalidCommentCases),
        MemberType = typeof(CreateReviewValidationTestCases)
    )]
    public async Task CreateReviewRequestValidator_Should_ReturnError_WhenCommentIsInvalid(string? comment)
    {
        // Arrange
        var validator = new CreateReviewRequestValidator();

        var request = new CreateReviewRequest
        {
            ProductId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Rating = 4,
            Comment = comment!,
        };

        // Act
        TestValidationResult<CreateReviewRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Comment).Only();
    }
}
