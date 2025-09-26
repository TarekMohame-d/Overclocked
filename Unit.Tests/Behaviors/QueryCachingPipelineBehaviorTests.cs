using Application.Abstraction.Messaging;
using Application.Common.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Application.Abstraction.Behaviors;
using Microsoft.Extensions.Logging.Abstractions;
using Application.Abstraction.Services;

namespace Unit.Tests.Behaviors;

public class QueryCachingPipelineBehaviorTests
{
    private readonly ICacheService _cacheServiceMock;
    private readonly ILogger<QueryCachingPipelineBehavior<TestQuery, Result<string>>> _loggerMock;
    private readonly QueryCachingPipelineBehavior<TestQuery, Result<string>> _behavior;

    public QueryCachingPipelineBehaviorTests()
    {
        _cacheServiceMock = Substitute.For<ICacheService>();
        _loggerMock = Substitute.For<ILogger<QueryCachingPipelineBehavior<TestQuery, Result<string>>>>();
        _behavior = new QueryCachingPipelineBehavior<TestQuery, Result<string>>(_cacheServiceMock, _loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldReturnCachedResult_WhenCacheHit()
    {
        // Arrange
        var query = new TestQuery();
        var cachedResult = Result<string>.Success("cached-value");

        _cacheServiceMock.GetAsync<Result<string>>(query.CacheKey)
            .Returns(cachedResult);

        // Act
        var result = await _behavior.Handle(query, _ => throw new Exception("Handler should not be called"), CancellationToken.None);

        // Assert
        result.ShouldBe(cachedResult);
        await _cacheServiceMock.Received(1)
            .GetAsync<Result<string>>(query.CacheKey);
        await _cacheServiceMock.DidNotReceive()
            .SetAsync(Arg.Any<string>(), Arg.Any<Result<string>>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldCallHandlerAndCacheResult_WhenCacheMiss()
    {
        // Arrange
        var query = new TestQuery();
        _cacheServiceMock.GetAsync<Result<string>>(query.CacheKey)
            .Returns((Result<string>?)null);

        var handlerResult = Result<string>.Success("from-handler");

        // Act
        var result = await _behavior.Handle(query, _ => Task.FromResult(handlerResult), CancellationToken.None);

        // Assert
        result.ShouldBe(handlerResult);

        await _cacheServiceMock.Received(1).SetAsync(
            query.CacheKey,
            handlerResult,
            query.SlidingExpiration,
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldBypassCache_WhenBypassCacheIsTrue()
    {
        // Arrange
        var query = new TestQuery { BypassCache = true };
        var handlerResult = Result<string>.Success("no-cache");

        // Act
        var result = await _behavior.Handle(query, _ => Task.FromResult(handlerResult), CancellationToken.None);

        // Assert
        result.ShouldBe(handlerResult);

        await _cacheServiceMock.DidNotReceive().GetAsync<Result<string>>(query.CacheKey);
        await _cacheServiceMock.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<Result<string>>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldNotCache_WhenHandlerResultFails()
    {
        // Arrange
        var query = new TestQuery();
        _cacheServiceMock.GetAsync<Result<string>>(query.CacheKey)
            .Returns((Result<string>?)null);

        var failedResult = Result<string>.Failure(new Error("some-error", ErrorType.NotFound, "not found"));

        // Act
        var result = await _behavior.Handle(query, _ => Task.FromResult(failedResult), CancellationToken.None);

        // Assert
        result.ShouldBe(failedResult);

        await _cacheServiceMock.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<Result<string>>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>());
    }

    // Helper test query
    public class TestQuery : ICachedQuery<Result<string>>
    {
        public string CacheKey => "TestCacheKey";
        public string? CacheSetKey => null;
        public bool BypassCache { get; set; }
        public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
    }
}
