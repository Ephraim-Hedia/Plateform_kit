using FluentAssertions;
using Platform.Application.Common.Behaviors;
using Platform.Domain.Results;
using Platform.UnitTests.Application.Common.Behaviors.TestDoubles;

namespace Platform.UnitTests.Application.Common.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_Should_CallNext_When_NoValidatorsRegistered()
    {
        var behavior = new ValidationBehavior<TestCommand, Result>([]);
        var nextCalled = false;

        var result = await behavior.Handle(new TestCommand("Ephraim"), Next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        return;

        Task<Result> Next(CancellationToken _)
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        }
    }

    [Fact]
    public async Task Handle_Should_CallNext_When_ValidationSucceeds()
    {
        var behavior = new ValidationBehavior<TestCommand, Result>([new TestCommandValidator()]);
        var nextCalled = false;

        var result = await behavior.Handle(new TestCommand("Ephraim"), Next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        return;

        Task<Result> Next(CancellationToken _)
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        }
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationResult_When_ValidationFails_For_Result()
    {
        var behavior = new ValidationBehavior<TestCommand, Result>([new TestCommandValidator()]);
        var nextCalled = false;

        var result = await behavior.Handle(new TestCommand(string.Empty), Next, CancellationToken.None);

        nextCalled.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Should().BeOfType<ValidationResult>()
            .Which.Errors.Should().NotBeEmpty();
        return;

        Task<Result> Next(CancellationToken _)
        {
            nextCalled = true;
            return Task.FromResult(Result.Success());
        }
    }

    [Fact]
    public async Task Handle_Should_ReturnValidationResultOfT_When_ValidationFails_For_ResultOfT()
    {
        var behavior = new ValidationBehavior<TestQuery, Result<string>>([new TestQueryValidator()]);
        var nextCalled = false;

        var result = await behavior.Handle(new TestQuery(string.Empty), Next, CancellationToken.None);

        nextCalled.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Should().BeOfType<ValidationResult<string>>()
            .Which.Errors.Should().NotBeEmpty();
        return;

        Task<Result<string>> Next(CancellationToken _)
        {
            nextCalled = true;
            return Task.FromResult(Result.Success("value"));
        }
    }
}
