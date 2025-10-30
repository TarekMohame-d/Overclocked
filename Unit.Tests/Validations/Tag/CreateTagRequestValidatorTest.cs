using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Validations;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Tag.TestCases;
using TagEntity = Domain.Entities.Tag;

namespace Unit.Tests.Validations.Tag;

public class CreateTagRequestValidatorTest
{
    private readonly ITagRepository _tagRepositoryMock;

    public CreateTagRequestValidatorTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
    }

    [Theory]
    [MemberData(nameof(CreateTagValidationTestCases.InvalidNameCases), MemberType = typeof(CreateTagValidationTestCases))]
    public async Task CreateTagRequestValidator_Should_ReturnError_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateTagRequestValidator(_tagRepositoryMock);
        var command = new CreateTagRequest
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
    public async Task CreateTagRequestValidator_Should_ReturnError_When_NameAlreadyExists()
    {
        // Arrange
        var validator = new CreateTagRequestValidator(_tagRepositoryMock);
        var command = new CreateTagRequest
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
