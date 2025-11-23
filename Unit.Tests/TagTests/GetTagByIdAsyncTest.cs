using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Results;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Application.Services.Tag.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.TagTests;

public class GetTagByIdAsyncTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly TagService _tagService;

    public GetTagByIdAsyncTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _tagService = new TagService(_tagRepositoryMock, Substitute.For<IUnitOfWork>());
    }

    [Fact]
    public async Task GetTagByIdAsync_Should_ReturnTag_When_TagExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var request = new GetTagByIdRequest { Id = tagId };
        Tag tag = new TagFaker().Generate();
        TagResponse tagDto = tag.ToDto();

        _tagRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Tag, bool>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns(tag);

        // Act
        Result<TagResponse> result = await _tagService.GetTagByIdAsync(request, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();

        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        tagDto.ShouldBeEquivalentTo(result.Data);

        await _tagRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Tag, bool>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetTagByIdAsync_Should_ReturnFailure_When_TagDoesNotExists()
    {
        // Arrange
        var tagId = Guid.CreateVersion7();
        var request = new GetTagByIdRequest { Id = tagId };

        _tagRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Tag, bool>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>())
            .Returns((Tag)null!);

        // Act
        Result<TagResponse> result = await _tagService.GetTagByIdAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Error.Type.ShouldBe(ErrorType.NotFound);

        await _tagRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<Tag, bool>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }
}
