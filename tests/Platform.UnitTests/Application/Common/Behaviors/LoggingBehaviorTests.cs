using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Platform.Application.Common.Behaviors;
using Platform.Domain.Results;
using Platform.UnitTests.Application.Common.Behaviors.TestDoubles;

namespace Platform.UnitTests.Application.Common.Behaviors;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_Should_ReturnResponse_When_NextSucceeds()
    {
        var logger = Substitute.For<ILogger<LoggingBehavior<TestCommand, Result>>>();
        var behavior = new LoggingBehavior<TestCommand, Result>(logger);

        var result = await behavior.Handle(
            new TestCommand("Ephraim"),
            _ => Task.FromResult(Result.Success()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_LogError_And_Rethrow_When_NextThrows()
    {
        var logger = Substitute.For<ILogger<LoggingBehavior<TestCommand, Result>>>();
        var behavior = new LoggingBehavior<TestCommand, Result>(logger);

        Task<Result> Next(CancellationToken _) => throw new InvalidOperationException("boom");

        var act = () => behavior.Handle(new TestCommand("Ephraim"), Next, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();

        logger.ReceivedCalls()
            .Should()
            .Contain(call =>
                call.GetMethodInfo().Name == nameof(ILogger.Log)
                && (LogLevel)call.GetArguments()[0]! == LogLevel.Error);
    }
}
