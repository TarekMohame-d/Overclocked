using FluentValidation.TestHelper;
using Overclocked.Application.Features.TagUseCases.UpdateTag;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.Unit.Tests.Validations.TagTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.TagTests;

public class UpdateTagRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateTagValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateTagValidationTestCases))]
    public void UpdateTagRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var tagId = TagId.Create();
        var request = new UpdateTagRequest { Id = tagId, Name = name! };

        var validator = new UpdateTagRequestValidator();

        // Act
        TestValidationResult<UpdateTagRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
