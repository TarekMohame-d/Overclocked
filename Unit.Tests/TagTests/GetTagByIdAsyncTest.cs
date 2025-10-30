using System.Net;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Features.Tag.Mapping;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.TagTests;

public class GetTagByIdAsyncTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ITagService _tagService;

    public GetTagByIdAsyncTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagService = new TagService(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task GetTagByIdAsync_Should_ReturnTag_When_TagExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var request = new GetTagByIdRequest { Id = tagId };
        var tag = new TagFaker().Generate();
        var tagDto = tag.ToDto();

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns(tag);

        // Act
        var result = await _tagService.GetTagByIdAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        tagDto.ShouldBeEquivalentTo(result.Data);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTagByIdAsync_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var request = new GetTagByIdRequest { Id = tagId };

        _tagRepositoryMock.GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        var result = await _tagService.GetTagByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .GetByIdAsync(Arg.Any<object[]>(), Arg.Any<CancellationToken>());
    }
}
