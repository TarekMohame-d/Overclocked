using Application.Services.ReviewReply.DTOs.Request;
using Application.Services.ReviewReply.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.ReviewReply.TestCases;

namespace Unit.Tests.Validations.ReviewReply;

public class CreateReviewReplyRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(CreateReviewReplyValidationTestCases.InvalidReplyCases), MemberType = typeof(CreateReviewReplyValidationTestCases))]
    public async Task CreateReviewReplyRequestValidator_Should_ReturnError_WhenReplyIsInvalid(string? reply)
    {
        // Arrange
        var validator = new CreateReviewReplyRequestValidator();

        var request = new CreateReviewReplyRequestBody
        {
            Reply = reply!
        };

        // Act
        TestValidationResult<CreateReviewReplyRequestBody> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Reply).Only();
    }
}
