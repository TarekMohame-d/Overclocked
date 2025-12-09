using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Tag.Commands;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate;
using Shouldly;

namespace Overclocked.Unit.Tests.TagTests;

public class CreateTagCommandHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly ITagCommands _tagCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public CreateTagCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();

        _tagCommands = new TagCommands(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateTagCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var command = new CreateTagCommand
        {
            Name = "Tag Name"
        };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.AddAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _tagCommands.CreateTagCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1)
            .AddAsync(Arg.Any<Tag>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
