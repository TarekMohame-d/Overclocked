using FluentValidation.TestHelper;
using Overclocked.Application.ReviewReply.Commands.UpdateReviewReply;
using Overclocked.Unit.Tests.Validations.ReviewReply.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewReply;

public class UpdateReviewReplyCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateReviewReplyValidationTestCases.InvalidReplyCases), MemberType = typeof(UpdateReviewReplyValidationTestCases))]
    public async Task UpdateReviewReplyCommandValidator_Should_ReturnError_WhenReplyIsInvalid(string? reply)
    {
        // Arrange
        var validator = new UpdateReviewReplyCommandValidator();

        var command = new UpdateReviewReplyCommand
        {
            Reply = reply!,
            EmployeeId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ReplyId = Guid.NewGuid()
        };

        // Act
        TestValidationResult<UpdateReviewReplyCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Reply).Only();
    }
}
