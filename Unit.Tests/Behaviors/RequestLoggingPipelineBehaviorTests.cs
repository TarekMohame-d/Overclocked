using Application.Abstraction.Behaviors;
using Application.Abstraction.Messaging;
using Application.Common.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.Behaviors;

public class RequestLoggingPipelineBehaviorTests
{
    private readonly ILogger<LoggingPipelineBehavior<TestRequest, Result<string>>> _loggerMock;
    private readonly LoggingPipelineBehavior<TestRequest, Result<string>> _behavior;

    public RequestLoggingPipelineBehaviorTests()
    {
        _loggerMock = Substitute.For<ILogger<LoggingPipelineBehavior<TestRequest, Result<string>>>>();
        _behavior = new LoggingPipelineBehavior<TestRequest, Result<string>>(_loggerMock);
    }

    [Fact]
    public async Task Handle_ShouldLogProcessing_AndCompleted_WhenSuccess()
    {
        // Arrange
        var request = new TestRequest();
        var expectedResult = Result<string>.Success("ok");

        // Act
        var result = await _behavior.Handle(request, _ => Task.FromResult(expectedResult), CancellationToken.None);

        // Assert
        result.ShouldBe(expectedResult);

        _loggerMock.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Processing request")),
            null,
            Arg.Any<Func<object, Exception?, string>>());

        _loggerMock.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Completed request")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task Handle_ShouldLogError_WhenFailure()
    {
        // Arrange
        var request = new TestRequest();
        var failedResult = Result<string>.Failure(new Error("err", ErrorType.Forbidden, "fail"));

        // Act
        var result = await _behavior.Handle(request, _ => Task.FromResult(failedResult), CancellationToken.None);

        // Assert
        result.ShouldBe(failedResult);

        _loggerMock.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Completed request")),
            null,
            Arg.Any<Func<object, Exception?, string>>());
    }

    public class TestRequest : IRequest { }
}
