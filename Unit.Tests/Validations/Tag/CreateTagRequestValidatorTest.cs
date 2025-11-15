using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Validations;
using FluentValidation.TestHelper;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Tag.TestCases;
using TagEntity = Domain.Entities.Tag;

namespace Unit.Tests.Validations.Tag;

public class CreateTagRequestValidatorTest
{
    private readonly ITagRepository _tagRepositoryMock = Substitute.For<ITagRepository>();

    [Theory]
    [MemberData(nameof(CreateTagValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateTagValidationTestCases))]
    public async Task CreateTagRequestValidator_Should_ReturnError_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateTagRequestValidator(_tagRepositoryMock);

        var request = new CreateTagRequest
        {
            Name = name!
        };

        _tagRepositoryMock.AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<CreateTagRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Fact]
    public async Task CreateTagRequestValidator_Should_ReturnError_When_NameAlreadyExists()
    {
        // Arrange
        var validator = new CreateTagRequestValidator(_tagRepositoryMock);

        var request = new CreateTagRequest
        {
            Name = "Tag Name"
        };

        _tagRepositoryMock.AnyAsync(Arg.Any<Expression<Func<TagEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<CreateTagRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count().ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
