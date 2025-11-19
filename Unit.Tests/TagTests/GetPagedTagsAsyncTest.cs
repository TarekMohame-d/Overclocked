using System.Net;
using Application.Abstraction.Repositories;
using Application.Common.Enums;
using Application.Common.Results;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Application.Services.Tag.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using MockQueryable;
using NSubstitute;
using Shouldly;
using SortDirection = Application.Common.Enums.SortDirection;

namespace Unit.Tests.TagTests;

public class GetPagedTagsAsyncTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly TagService _tagService;

    public GetPagedTagsAsyncTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _tagService = new TagService(_tagRepositoryMock, Substitute.For<IUnitOfWork>());
    }

    [Fact]
    public async Task GetPagedTagsAsync_Should_ReturnTags_WhenTagsExist()
    {
        // Arrange
        var request = new GetPagedTagsRequest { Page = 1, PageSize = 10 };
        List<Tag> tags = new TagFaker().Generate(3);

        var tagDtos = tags.ToDto().ToList();

        IQueryable<Tag> mockQueryable = tags.BuildMock();

        _tagRepositoryMock.GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<SortDirection>()).Returns(mockQueryable);

        // Act
        Result<PagedResult<TagListResponse>> result = await _tagService.GetPagedTagsAsync(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        tags.Count.ShouldBe(result.Data.Items.Count);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        tagDtos.ShouldBeEquivalentTo(result.Data.Items);

        _tagRepositoryMock.Received(1).GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<SortDirection>());
    }

    [Fact]
    public async Task GetPagedTagsAsync_Should_ReturnEmptyList_WhenTagsDoesNotExist()
    {
        // Arrange
        var request = new GetPagedTagsRequest();

        List<Tag> tags = [];
        IQueryable<Tag> mockQueryable = tags.BuildMock();

        _tagRepositoryMock.GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<SortDirection>()).Returns(mockQueryable);

        // Act
        Result<PagedResult<TagListResponse>> result = await _tagService.GetPagedTagsAsync(
            request,
            CancellationToken.None
        );

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count.ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _tagRepositoryMock.Received(1).GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<SortDirection>());
    }
}
