using System.Net;
using System.Net.Http.Json;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Response;
using Application.Services.Tag.Mapping;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Infrastructure.Data;
using Integration.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace Integration.Tests.TagTests;

[Collection(nameof(SharedTestCollection))]
public class GetTagByIdTest(CustomWebApplicationFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.HttpClient;

    public async Task InitializeAsync() => await factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetById_Should_ReturnFailure_WhenNotFound()
    {
        // Arrange
        var id = Guid.CreateVersion7();

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        Result<TagResponse>? result = await response.Content.ReadFromJsonAsync<Result<TagResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Data.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_IdIsMalformedGuid()
    {
        // Arrange
        const string WrongId = "abc";

        // Act
        HttpResponseMessage response = await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", WrongId));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_Should_ReturnTagFromDatabase_When_CacheMiss()
    {
        // Arrange
        Tag tag = await SeedDatabaseAsync();

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", tag.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<TagResponse>? result = await response.Content.ReadFromJsonAsync<Result<TagResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Id.ShouldBe(tag.Id);
        result.Data.Name.ShouldBe(tag.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnTagFromCache_WhenCacheHit()
    {
        // Arrange
        TagResponse tagDto = await SeedCacheAsync();

        // Act
        HttpResponseMessage response =
            await _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", tagDto.Id.ToString()));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        Result<TagResponse>? result = await response.Content.ReadFromJsonAsync<Result<TagResponse>>();

        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Data.ShouldNotBeNull();
        result.Data.Name.ShouldBe(tagDto.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnConsistentResults_When_CalledConcurrently()
    {
        // Arrange
        IEnumerable<Tag> tags = await SeedDatabaseRangeAsync();
        var ids = tags.Select(x => x.Id).ToList();
        const int ConcurrentCalls = 10;
        var rnd = new Random();
        var tasks = new List<Task<HttpResponseMessage>>();

        // Act
        for (var i = 0; i < ConcurrentCalls; i++)
        {
            Guid randomId = ids[rnd.Next(ids.Count)];
            Task<HttpResponseMessage> task =
                _client.GetAsync(TagRoutes.GetById.Replace("{id:guid}", randomId.ToString()));
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);

        // Assert
        foreach (Task<HttpResponseMessage> task in tasks)
        {
            HttpResponseMessage response = await task;
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            Result<TagResponse>? result = await response.Content.ReadFromJsonAsync<Result<TagResponse>>();

            result.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.StatusCode.ShouldBe(HttpStatusCode.OK);
            result.Data.ShouldNotBeNull();
        }
    }

    private async Task<Tag> SeedDatabaseAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Tag tag = new TagFaker().Generate();
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        return tag;
    }

    private async Task<IEnumerable<Tag>> SeedDatabaseRangeAsync(int count = 10)
    {
        using IServiceScope scope = factory.Services.CreateScope();

        List<Tag> tags = new TagFaker().Generate(count);
        ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Tags.AddRangeAsync(tags);
        await dbContext.SaveChangesAsync();

        return tags;
    }

    private async Task<TagResponse> SeedCacheAsync()
    {
        using IServiceScope scope = factory.Services.CreateScope();

        Tag tag = new TagFaker().Generate();

        ICacheService cache = scope.ServiceProvider.GetRequiredService<ICacheService>();
        var key = CacheKeys.Tag(tag.Id.ToString());
        TagResponse tagDto = tag.ToDto();
        var result = Result<TagResponse>.Success(tagDto);
        await cache.SetAsync(key, result, TimeSpan.FromMinutes(5));

        return tagDto;
    }
}
