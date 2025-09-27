using System.Net;
using Application.Common.Results;
using Application.Features.Tag.Commands.UpdateTag;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Domain.Repositories;
using Application.Abstraction.Messaging;
using Domain.Entities;
using Application.Abstraction.Services;

namespace Unit.Tests.TagTests.Commands;

public class UpdateTagCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly UpdateTagCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public UpdateTagCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _mediatorMock = Substitute.For<IMediator>();

        _handler = new UpdateTagCommandHandler(
            _unitOfWorkMock,
            _tagRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new UpdateTagWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "AMD"
        };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTagExist_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UpdateTagWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "AMD"
        };

        var tag = new TagFaker().Generate();

        tag.Name = command.Name;

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.Update(Arg.Any<Tag>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _tagRepositoryMock.Received(1)
            .Update(Arg.Any<Tag>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());
    }
}
