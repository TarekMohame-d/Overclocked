using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.TagUseCases.CreateTag;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.TagAggregate;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.TagTests;

public class CreateTagRequestHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly CreateTagRequestHandler _createTagRequestHandler;

    public CreateTagRequestHandlerTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();

        _createTagRequestHandler = new CreateTagRequestHandler(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task CreateTagRequestHandler_Should_ReturnFailure_When_NameAlreadyExists()
    {
        // Arrange
        var request = new CreateTagRequest { Name = "Tag Name" };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.NameExistsAsync(request.Name, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _createTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _tagRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _tagRepositoryMock.DidNotReceive().Add(Arg.Any<Tag>());

        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateTagRequestHandler_Should_ReturnFailure_When_NameIsInvalid()
    {
        // Arrange
        var request = new CreateTagRequest { Name = "    " };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.NameExistsAsync(request.Name, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _createTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _tagRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _tagRepositoryMock.DidNotReceive().Add(Arg.Any<Tag>());

        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateTagRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new CreateTagRequest { Name = "Tag Name" };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.NameExistsAsync(request.Name, Arg.Any<CancellationToken>()).Returns(false);

        _tagRepositoryMock.Add(Arg.Any<Tag>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _createTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _tagRepositoryMock.Received(1).Add(Arg.Any<Tag>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
