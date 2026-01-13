using FluentValidation.TestHelper;
using Overclocked.Application.Features.ReviewReplyUseCases.UpdateReviewReply;
using Overclocked.Unit.Tests.Validations.ReviewReplyTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewReplyTests;

public class UpdateReviewReplyRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateReviewReplyValidationTestCases.InvalidReplyCases),
        MemberType = typeof(UpdateReviewReplyValidationTestCases)
    )]
    public async Task UpdateReviewReplyRequestValidator_Should_ReturnError_WhenReplyIsInvalid(string? reply)
    {
        // Arrange
        var validator = new UpdateReviewReplyRequestValidator();

        var request = new UpdateReviewReplyRequest
        {
            Reply = reply!,
            EmployeeId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<UpdateReviewReplyRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Reply).Only();
    }
}
