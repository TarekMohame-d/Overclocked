using System.Net;
using Application.Features.Tag.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;
using MockQueryable;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Services.Tag;
using Application.Services.Tag.DTOs.Request;
using Application.Common.Enums;

namespace Unit.Tests.TagTests;

public class GetPagedTagsAsyncTest
{
    private readonly ITagRepository _tagRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ITagService _tagService;

    public GetPagedTagsAsyncTest()
    {
        _tagRepositoryMock = Substitute.For<ITagRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tagService = new TagService(_tagRepositoryMock, _unitOfWorkMock);
    }

    [Fact]
    public async Task GetPagedTagsAsync_Should_ReturnTags_WhenTagsExist()
    {
        // Arrange
        var query = new GetPagedTagsQuery
        {
            Page = 1,
            PageSize = 10
        };
        var tags = new TagFaker().Generate(3);

        var tagDtos = tags.ToDto().ToList();

        var mockQueryable = tags.BuildMock();

        _tagRepositoryMock.GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<Application.Common.Enums.SortDirection>())
            .Returns(mockQueryable);

        // Act
        var result = await _tagService.GetPagedTagsAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        tags.Count.ShouldBe(result.Data.Items.Count());
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        tagDtos.ShouldBeEquivalentTo(result.Data.Items);

        _tagRepositoryMock.Received(1)
            .GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<Application.Common.Enums.SortDirection>());
    }

    [Fact]
    public async Task GetPagedTagsAsync_Should_ReturnEmptyList_WhenTagsDoesNotExist()
    {
        // Arrange
        var query = new GetPagedTagsQuery();

        _tagRepositoryMock.GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<Application.Common.Enums.SortDirection>())
            .Returns(new List<Tag>().BuildMock());

        // Act
        var result = await _tagService.GetPagedTagsAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Data.ShouldNotBeNull();
        result.Error.ShouldBeNull();
        result.Data.Items.ShouldBeEmpty();
        result.Data.Items.Count().ShouldBe(0);
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        _tagRepositoryMock.Received(1)
            .GetTagsQuery(Arg.Any<TagSortField>(), Arg.Any<Application.Common.Enums.SortDirection>());
    }
}
