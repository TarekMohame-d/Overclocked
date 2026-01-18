using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.Application.Features.TagUseCases.Mapping;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.TagAggregate;
using Overclocked.Infrastructure.Persistence;
using Overclocked.Integration.Tests.Shared;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Integration.Tests.TagTests;

public class GetPagedTagsTest(ApiTestFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.HttpClient;

    public async ValueTask InitializeAsync() => await fixture.ResetDatabaseAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task GetPagedTags_Should_ReturnFromDatabase_When_ThereIsDataAndCacheMiss()
    {
        // Arrange
        IEnumerable<Tag> tags = await SeedDatabaseAsync();

        var searchTerm = tags.First().Name;

        var count = tags.Count(x => x.Name.Contains(searchTerm));

        // Act
        var url = $"{TagRoutes.GetPaged}?Page=1&PageSize=10&SearchTerm={searchTerm}&SortBy=name&Direction=asc";
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<TagPagedResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<TagPagedResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count().ShouldBe(count);
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnFromCache_When_ThereIsDataAndCacheHit()
    {
        // Arrange
        IEnumerable<TagPagedResponse> tagListDtos = await SeedCacheAsync();

        var searchTerm = tagListDtos.First().Name;

        var count = tagListDtos.Count(x => x.Name.Contains(searchTerm));

        // Act
        var url = $"{TagRoutes.GetPaged}?Page=1&PageSize=20&SearchTerm={searchTerm}&SortBy=Id&Direction=Asc";
        HttpResponseMessage response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<TagPagedResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<TagPagedResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.Count().ShouldBe(count);
    }

    [Fact]
    public async Task GetPagedTags_Should_ReturnEmptyList_When_ThereIsNoData()
    {
        // Arrange

        // Act
        const string Url = $"{TagRoutes.GetPaged}?Page=1&PageSize=10&SearchTerm=test&SortBy=name&Direction=asc";
        HttpResponseMessage response = await _client.GetAsync(Url);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        PagedResult<TagPagedResponse>? result = await response.Content.ReadFromJsonAsync<PagedResult<TagPagedResponse>>();

        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.Items.ShouldBeEmpty();
        result.Items.Count().ShouldBe(0);
    }

    private async Task<IEnumerable<Tag>> SeedDatabaseAsync()
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        List<Tag> tags = new TagFaker().Generate(20);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return tags;
    }

    private async Task<IEnumerable<TagPagedResponse>> SeedCacheAsync()
    {
        using IServiceScope scope = fixture.Services.CreateScope();

        List<Tag> tags = new TagFaker().Generate(20);
        var searchTerm = tags[0].Name;

        var cachedTags = tags.Where(x => x.Name.Contains(searchTerm)).ToList();

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.TagPaged(1, 20, searchTerm, "id", "asc");

        IEnumerable<TagPagedResponse> tagListDtos = cachedTags.ToDto();
        var pagedResult = PagedResult<TagPagedResponse>.Create(tagListDtos, 1, 20, tagListDtos.Count());

        await cache.SetAsync(key, pagedResult, TimeSpan.FromMinutes(5));

        return tagListDtos;
    }
}
