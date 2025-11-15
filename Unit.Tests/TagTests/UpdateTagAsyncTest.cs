using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.TagTests;

public class UpdateTagAsyncTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly TagService _tagService;
    private readonly IUnitOfWork _unitOfWorkMock;

    public UpdateTagAsyncTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagService = new TagService(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task UpdateTagAsync_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var request = new UpdateTagRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Tag Name"
        };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        Result result = await _tagService.UpdateTagAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateTagAsync_When_TagExist_Should_ReturnSuccess()
    {
        // Arrange
        var request = new UpdateTagRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Tag Name"
        };

        Tag tag = new TagFaker().Generate();

        tag.Name = request.Name;

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        _tagRepositoryMock.Update(Arg.Any<Tag>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _tagService.UpdateTagAsync(request, CancellationToken.None);

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
    }
}
