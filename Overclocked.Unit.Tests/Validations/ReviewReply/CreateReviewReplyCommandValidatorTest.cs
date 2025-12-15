using FluentValidation.TestHelper;
using Overclocked.Application.ReviewReply.Commands.CreateReviewReply;
using Overclocked.Unit.Tests.Validations.ReviewReply.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewReply;

public class CreateReviewReplyCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(CreateReviewReplyValidationTestCases.InvalidReplyCases), MemberType = typeof(CreateReviewReplyValidationTestCases))]
    public async Task CreateReviewReplyCommandValidator_Should_ReturnError_WhenReplyIsInvalid(string? reply)
    {
        // Arrange
        var validator = new CreateReviewReplyCommandValidator();

        var command = new CreateReviewReplyCommand
        {
            Reply = reply!,
            EmployeeId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid()
        };

        // Act
        TestValidationResult<CreateReviewReplyCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Reply).Only();
    }
}
