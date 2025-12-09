using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Unit.Tests.Validations.Tag.TestCases;
using Shouldly;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Unit.Tests.Validations.Tag;

public class CreateTagCommandValidatorTest
{
    private readonly ITagRepository _tagRepositoryMock = Substitute.For<ITagRepository>();

    [Theory]
    [MemberData(
        nameof(CreateTagValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateTagValidationTestCases))]
    public async Task CreateTagCommandValidator_Should_ReturnError_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateTagCommandValidator(_tagRepositoryMock);

        var command = new CreateTagCommand
        {
            Name = name!
        };

        _tagRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateTagCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task CreateTagCommandValidator_Should_ReturnError_When_NameAlreadyExists()
    {
        // Arrange
        var validator = new CreateTagCommandValidator(_tagRepositoryMock);

        var command = new CreateTagCommand
        {
            Name = "Tag Name"
        };

        _tagRepositoryMock
            .AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateTagCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
