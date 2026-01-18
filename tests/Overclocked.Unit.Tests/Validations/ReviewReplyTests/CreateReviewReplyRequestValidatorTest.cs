using FluentValidation.TestHelper;
using Overclocked.Application.Features.ReviewReplyUseCases.CreateReviewReply;
using Overclocked.Unit.Tests.Validations.ReviewReplyTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.ReviewReplyTests;

public class CreateReviewReplyRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(CreateReviewReplyValidationTestCases.InvalidReplyCases),
        MemberType = typeof(CreateReviewReplyValidationTestCases)
    )]
    public async Task CreateReviewReplyRequestValidator_Should_ReturnError_WhenReplyIsInvalid(string? reply)
    {
        // Arrange
        var validator = new CreateReviewReplyRequestValidator();

        var request = new CreateReviewReplyRequest
        {
            Reply = reply!,
            EmployeeId = Guid.NewGuid(),
            ReviewId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<CreateReviewReplyRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Reply).Only();
    }
}
