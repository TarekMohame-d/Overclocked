using FluentValidation.TestHelper;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.Unit.Tests.Validations.Tag.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Tag;

public class UpdateTagCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateTagValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateTagValidationTestCases))]
    public void UpdateTagCommandValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var tagId = TagId.Create();
        var request = new UpdateTagCommand
        {
            Id = tagId,
            Name = name!
        };

        var validator = new UpdateTagCommandValidator();

        // Act
        TestValidationResult<UpdateTagCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
