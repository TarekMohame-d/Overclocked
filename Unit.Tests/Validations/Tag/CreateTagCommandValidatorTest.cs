using System.Linq.Expressions;
using Application.Features.Tag.Commands.CreateTag;
using Domain.Repositories;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Tag.TestCases;
using TagEntity = Domain.Entities.Tag;

namespace Unit.Tests.Validations.Tag;

public class CreateTagCommandValidatorTest
{
    private readonly ITagRepository _tagRepositoryMock;

    public CreateTagCommandValidatorTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
    }

    [Theory]
    [MemberData(nameof(CreateTagValidationTestCases.InvalidNameCases), MemberType = typeof(CreateTagValidationTestCases))]
    public async Task TagValidator_WhenNameIsInvalid_ShouldReturnError(string? name)
    {
        // Arrange
        var validator = new CreateTagCommandValidator(_tagRepositoryMock);
        var command = new CreateTagCommand
        {
            Name = name!
        };

        _tagRepositoryMock.AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
        if (!string.IsNullOrWhiteSpace(name))
            await _tagRepositoryMock.Received(1)
                .AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task TagValidator_WhenNameAlreadyExists_ShouldReturnError()
    {
        // Arrange
        var validator = new CreateTagCommandValidator(_tagRepositoryMock);
        var command = new CreateTagCommand
        {
            Name = "Nike"
        };

        _tagRepositoryMock.AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();

        await _tagRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
