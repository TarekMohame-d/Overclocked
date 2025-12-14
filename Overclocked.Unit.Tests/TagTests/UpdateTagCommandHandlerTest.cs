using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.TagTests;

public class UpdateTagCommandHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly UpdateTagCommandHandler _updateTagCommandHandler;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateTagCommandHandlerTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _updateTagCommandHandler = new UpdateTagCommandHandler(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateTagCommandHandler_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand
        {
            Id = tagId,
            Name = "Tag Name"
        };

        _tagRepositoryMock.FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        Result result = await _updateTagCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagCommandHandler_Should_ReturnSuccess_When_TagExistAndNameIsSame()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand
        {
            Id = tagId,
            Name = "Tag Name"
        };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _updateTagCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1)
            .FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagCommandHandler_Should_ReturnFailure_When_TagExistAndNewNameAlreadyExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand
        {
            Id = tagId,
            Name = "Tag Name"
        };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Tag, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        Result result = await _updateTagCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _tagRepositoryMock.Received(1)
            .FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _tagRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Tag, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagCommandHandler_Should_ReturnSuccess_When_TagExistAndNameChangedAndNameIsUnique()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var command = new UpdateTagCommand
        {
            Id = tagId,
            Name = "Tag Name"
        };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.AnyAsync(Arg.Any<Expression<Func<Tag, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _updateTagCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1)
            .FindAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _tagRepositoryMock.Received(1)
            .AnyAsync(Arg.Any<Expression<Func<Tag, bool>>>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
