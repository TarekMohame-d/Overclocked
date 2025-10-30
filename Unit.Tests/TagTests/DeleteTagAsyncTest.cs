using System.Net;
using Application.Common.Results;
using ArchitectureTests.FakeData;
using NSubstitute;
using Shouldly;
using Application.Abstraction.Messaging;
using Domain.Entities;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;

namespace Unit.Tests.TagTests;

public class DeleteTagAsyncTest
{
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ITagRepository _tagRepositoryMock;
    private readonly ITagService _tagService;

    public DeleteTagAsyncTest()
    {
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _tagService = new TagService(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task DeleteTagAsync_Should_ReturnFailure_WhenTagDoesNotExists()
    {
        // Arrange
        var request = new DeleteTagRequest
        {
            Id = Guid.CreateVersion7()
        };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Tag?>(null));

        // Act
        var result = await _tagService.DeleteTagAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteTagAsync_Should_ReturnSuccess_WhenTagExists()
    {
        // Arrange
        var request = new DeleteTagRequest
        {
            Id = Guid.CreateVersion7()
        };

        var tag = new TagFaker().Generate();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.Delete(Arg.Any<Tag>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        var result = await _tagService.DeleteTagAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        _tagRepositoryMock.Received(1)
            .Delete(Arg.Any<Tag>());
    }
}
