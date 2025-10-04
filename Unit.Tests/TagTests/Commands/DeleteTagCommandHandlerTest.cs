using System.Net;
using Application.Common.Results;
using Application.Features.Tag.Commands.DeleteTag;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Domain.Entities;
using Application.Abstraction.Repositories;

namespace Unit.Tests.TagTests.Commands;

public class DeleteTagCommandHandlerTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly DeleteTagCommandHandler _handler;
    private readonly IMediator _mediatorMock;

    public DeleteTagCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _mediatorMock = Substitute.For<IMediator>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _handler = new DeleteTagCommandHandler(
            _unitOfWorkMock,
            _tagRepositoryMock,
            _mediatorMock);
    }

    [Fact]
    public async Task Handle_WhenTagDoesNotExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new DeleteTagCommand
        {
            Id = Guid.CreateVersion7()
        };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Tag?>(null));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTagExists_ShouldReturnSuccess()
    {
        // Arrange
        var command = new DeleteTagCommand
        {
            Id = Guid.CreateVersion7()
        };

        var tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.Delete(Arg.Any<Tag>());

        _mediatorMock.Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _mediatorMock.Received(1)
            .Publish(Arg.Any<INotification>(), Arg.Any<CancellationToken>());

        _tagRepositoryMock.Received(1)
            .Delete(Arg.Any<Tag>());
    }
}
