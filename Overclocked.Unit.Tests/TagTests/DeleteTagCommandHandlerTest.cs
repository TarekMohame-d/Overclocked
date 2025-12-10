using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Tag.Commands;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Shouldly;

namespace Overclocked.Unit.Tests.TagTests;

public class DeleteTagCommandHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly ITagCommands _tagCommands;
    private readonly IUnitOfWork _unitOfWorkMock;

    public DeleteTagCommandHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _tagCommands = new TagCommands(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteTagCommandHandler_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();

        var command = new DeleteTagCommand
        {
            Id = tagId
        };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        Result result = await _tagCommands.DeleteTagCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTagCommandHandler_Should_ReturnSuccess_When_TagExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var command = new DeleteTagCommand
        {
            Id = tagId
        };

        Tag? tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.Delete(Arg.Any<Tag>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _tagCommands.DeleteTagCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());

        _tagRepositoryMock.Received(1)
            .Delete(Arg.Any<Tag>());
    }
}
