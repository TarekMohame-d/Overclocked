using FluentValidation.TestHelper;
using Overclocked.Application.Features.TagUseCases.CreateTag;
using Overclocked.Unit.Tests.Validations.TagTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.TagTests;

public class CreateTagRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(CreateTagValidationTestCases.InvalidNameCases), MemberType = typeof(CreateTagValidationTestCases))]
    public async Task CreateTagRequestValidator_Should_ReturnError_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateTagRequestValidator();

        var request = new CreateTagRequest { Name = name! };

        // Act
        TestValidationResult<CreateTagRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
