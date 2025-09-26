using System.Net;
using System.Net.Http.Json;
using Api.Common.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Tag.Mapping;
using Application.Features.Tag.Queries.GetTagById;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.TagTests.Queries;

[Collection(nameof(SharedTestCollection))]
public class GetTagByIdTest : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GetTagByIdTest(CustomWebApplicationFactory factory)
    {
        _client = factory.HttpClient;
        _factory = factory;
    }

    public async Task InitializeAsync() => await _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_WhenNotFound_ShouldReturnFailure()
    {
        // Arrange
        Guid id = Guid.CreateVersion7();

        // Act
        var response = await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        var result = await response.Content.ReadFromJsonAsync<Result<TagDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetById_WhenIdIsMalformedGuid_ShouldReturnNotFound()
    {
        // Arrange
        var wrongId = "abc";

        // Act
        var response = await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", wrongId.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_WhenCacheMiss_ShouldReturnTagFromDatabase()
    {
        // Arrange
        var tag = await SeedDatabaseAsync();

        // Act
        var response = await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", tag.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<TagDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(tag.Id);
        result.Data.Name.ShouldBe(tag.Name);
    }

    [Fact]
    public async Task GetById_WhenCacheHit_ShouldReturnTagFromCache()
    {
        // Arrange
        var tagDto = await SeedCacheAsync();

        // Act
        var response = await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", tagDto.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<Result<TagDto>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(tagDto.Name);
    }

    [Fact]
    public async Task GetById_WhenCalledConcurrently_ShouldReturnConsistentResults()
    {
        // Arrange
        var tags = await SeedDatabaseRangeAsync(10);
        var ids = tags.Select(x => x.Id).ToList();
        int concurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (int i = 0; i < concurrentCalls; i++)
        {
            var randomId = ids[rnd.Next(ids.Count)];
            var task = _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", randomId.ToString()));
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (var task in tasks)
        {
            var response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<Result<TagDto>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Tag> SeedDatabaseAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var tag = new TagFaker().Generate();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return tag;
    }

    private async Task<IEnumerable<Tag>> SeedDatabaseRangeAsync(int count = 10)
    {
        using var scope = _factory.Services.CreateScope();

        var tags = new TagFaker().Generate(count);
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return tags;
    }

    private async Task<TagDto> SeedCacheAsync()
    {
        using var scope = _factory.Services.CreateScope();

        var tag = new TagFaker().Generate();

        var cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Tag(tag.Id.ToString());
        var tagDto = tag.ToDto();
        var result = Result<TagDto>.Success(tagDto);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return tagDto;
    }
}
