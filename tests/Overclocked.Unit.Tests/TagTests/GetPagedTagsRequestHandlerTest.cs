using NSubstitute;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.Application.Features.TagUseCases.GetPagedTags;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.TagAggregate;
using Overclocked.SharedKernel;
using Shouldly;
using SortDirection = Overclocked.Application.Common.Enums.SortDirection;

namespace Overclocked.Unit.Tests.TagTests;

public class GetPagedTagsRequestHandlerTest
{
    private readonly ITagReadRepository _tagReadRepositoryMock;
    private readonly GetPagedTagsRequestHandler _getPagedTagsRequestHandler;

    public GetPagedTagsRequestHandlerTest()
    {
        _tagReadRepositoryMock = Substitute.For<ITagReadRepository>();
        _getPagedTagsRequestHandler = new GetPagedTagsRequestHandler(_tagReadRepositoryMock);
    }

    [Fact]
    public async Task GetPagedTagsRequestHandler_Should_ReturnEmptyList_When_NoTagsFoundBySearchTerm()
    {
        // Arrange
        var request = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = "search term",
            SortBy = "id",
            Direction = "asc",
        };

        _tagReadRepositoryMock.CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(0);

        // Act
        Result<PagedResult<TagPagedResponse>> result = await _getPagedTagsRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Items.ShouldBeEmpty();
        result.Value.HasNextPage.ShouldBeFalse();
        result.Error.ShouldBe(Error.None);

        await _tagReadRepositoryMock.Received(1).CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPagedTagsAsync_Should_ReturnTags_When_TagsFoundWithSearchTerm()
    {
        // Arrange
        List<Tag> tags = new TagFaker().Generate(3);
        var request = new GetPagedTagsRequest
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = tags[0].Name,
            SortBy = "id",
            Direction = "asc",
        };

        _tagReadRepositoryMock.CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(10);

        _tagReadRepositoryMock
            .GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<TagSortField>(), Arg.Any<SortDirection>())
            .Returns(tags);

        // Act
        Result<PagedResult<TagPagedResponse>> result = await _getPagedTagsRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Error.ShouldBe(Error.None);
        result.Value.Items.ShouldNotBeEmpty();

        await _tagReadRepositoryMock.Received(1).CountAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _tagReadRepositoryMock
            .Received(1)
            .GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(), Arg.Any<TagSortField>(), Arg.Any<SortDirection>());
    }
}
