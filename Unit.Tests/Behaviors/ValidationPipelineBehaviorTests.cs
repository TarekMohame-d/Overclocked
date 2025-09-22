using Application.Abstraction.Behaviors;
using Application.Abstraction.Messaging;
using Application.Common.Results;
using FluentValidation;
using FluentValidation.Results;
using Shouldly;

namespace Unit.Tests.Behaviors;

public class ValidationPipelineBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidators()
    {
        // Arrange
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(Array.Empty<IValidator<TestCommand>>());
        var expected = Result<string>.Success("ok");

        // Act
        var result = await behavior.Handle(new TestCommand(),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
    {
        // Arrange
        var validator = new AlwaysValidValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(new[] { validator });
        var expected = Result<string>.Success("valid");

        // Act
        var result = await behavior.Handle(new TestCommand(),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        // Assert
        result.ShouldBe(expected);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenValidationFails()
    {
        // Arrange
        var validator = new AlwaysFailValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(new[] { validator });

        // Act
        var result = await behavior.Handle(new TestCommand(),
            _ => throw new Exception("Should not be called"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Validation);
        result.Error.ValidationErrors!.ShouldContainKey("Value");
        result.Error.ValidationErrors!["Value"][0].ShouldBe("Invalid value");
    }

    [Fact]
    public async Task Handle_ShouldAggregateFailures_FromMultipleValidators()
    {
        // Arrange
        var validator1 = new AlwaysFailValidator();
        var validator2 = new AnotherFailValidator();
        var behavior = new ValidationPipelineBehavior<TestCommand, Result<string>>(new IValidator<TestCommand>[] { validator1, validator2 });

        // Act
        var result = await behavior.Handle(new TestCommand(),
            _ => throw new Exception("Should not be called"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Validation);
        result.Error.ValidationErrors!.ShouldContainKey("Value");
        result.Error.ValidationErrors!.ShouldContainKey("Other");
    }

    // Test command
    public class TestCommand : ICommand<Result<string>> { }

    // Validators
    private class AlwaysValidValidator : AbstractValidator<TestCommand>
    {
        public AlwaysValidValidator() => RuleFor(x => x).NotNull(); // Always passes
    }

    private class AlwaysFailValidator : AbstractValidator<TestCommand>
    {
        public AlwaysFailValidator() => RuleFor(x => x).Custom((_, context) =>
        {
            context.AddFailure(new ValidationFailure("Value", "Invalid value"));
        });
    }

    private class AnotherFailValidator : AbstractValidator<TestCommand>
    {
        public AnotherFailValidator() => RuleFor(x => x).Custom((_, context) =>
        {
            context.AddFailure(new ValidationFailure("Other", "Another error"));
        });
    }
}
