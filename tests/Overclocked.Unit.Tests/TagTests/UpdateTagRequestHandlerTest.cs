using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.TagUseCases.UpdateTag;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.TagTests;

public class UpdateTagRequestHandlerTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly UpdateTagRequestHandler _updateTagRequestHandler;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateTagRequestHandlerTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _updateTagRequestHandler = new UpdateTagRequestHandler(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateTagRequestHandler_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var request = new UpdateTagRequest { Id = tagId, Name = "Tag Name" };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns((Tag)null!);

        // Act
        Result result = await _updateTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagRequestHandler_Should_ReturnSuccess_When_TagExistAndNameIsSame()
    {
        // Arrange
        Tag tag = new TagFaker().Generate();
        var request = new UpdateTagRequest { Id = tag.Id.Value, Name = tag.Name };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(tag);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _updateTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagRequestHandler_Should_ReturnFailure_When_TagExistAndNewNameAlreadyExists()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var request = new UpdateTagRequest { Id = tagId, Name = "Tag Name" };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(tag);

        _tagRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _updateTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _tagRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagRequestHandler_Should_ReturnFailure_When_TagExistAndNewNameIsInvalid()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var request = new UpdateTagRequest { Id = tagId, Name = "    " };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(tag);

        _tagRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _tagRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagRequestHandler_Should_ReturnSuccess_When_TagExistAndNewNameChangedIsUnique()
    {
        // Arrange
        var tagId = Guid.NewGuid();
        var request = new UpdateTagRequest { Id = tagId, Name = "Tag Name" };

        Tag tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>()).Returns(tag);

        _tagRepositoryMock.NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _updateTagRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _tagRepositoryMock.Received(1).GetByIdAsync(Arg.Any<TagId>(), Arg.Any<CancellationToken>());

        await _tagRepositoryMock.Received(1).NameExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
