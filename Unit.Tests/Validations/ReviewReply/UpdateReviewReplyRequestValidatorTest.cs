using Application.Services.ReviewReply.DTOs.Request;
using Application.Services.ReviewReply.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.ReviewReply.TestCases;

namespace Unit.Tests.Validations.ReviewReply;

public class UpdateReviewReplyRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateReviewReplyValidationTestCases.InvalidReplyCases), MemberType = typeof(UpdateReviewReplyValidationTestCases))]
    public async Task CreateReviewReplyRequestValidator_Should_ReturnError_WhenReplyIsInvalid(string? reply)
    {
        // Arrange
        var validator = new UpdateReviewReplyRequestValidator();

        var request = new UpdateReviewReplyRequestBody
        {
            Reply = reply!
        };

        // Act
        TestValidationResult<UpdateReviewReplyRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Reply).Only();
    }
}
