using System.Net;
using Application.Abstraction.Messaging;
using Application.Features.Tag.Commands.CreateTag;
using ArchitectureTests.FakeData;
using Domain.Repositories;
using NSubstitute;
using Shouldly;
using Domain.Entities;
using Application.Abstraction.Services;

namespace Unit.Tests.TagTests.Commands;

public class CreateTagCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly CreateTagCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public CreateTagCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _mediatorMock = Substitute.For<IMediator>();
        _handler = new CreateTagCommandHandler(
            _tagRepositoryMock,
            _unitOfWorkMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenThereIsNoError_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreateTagCommand
        {
            Name = "Test"
        };

        var tag = new TagFaker().Generate();

        _tagRepositoryMock.AddAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _tagRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
