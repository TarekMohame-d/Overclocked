using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.TagUseCases.DeleteTag;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.TagTests;

public class DeleteTagRequestHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly DeleteTagRequestHandler _deleteTagRequestHandler;

    public DeleteTagRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _deleteTagRequestHandler = new DeleteTagRequestHandler(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteTagRequestHandler_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();

        var request = new DeleteTagRequest { Id = tagId };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns((Tag)null!);

        // Act
        Result result = await _deleteTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTagRequestHandler_Should_ReturnSuccess_When_TagExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var request = new DeleteTagRequest { Id = tagId };

        Tag? tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(tag);

        _tagRepositoryMock.Remove(Arg.Any<Tag>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _deleteTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());

        _tagRepositoryMock.Received(1).Remove(Arg.Any<Tag>());
    }
}
