using FluentAssertions;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.UnitTests.Domain.Results;

public class ResultTests
{
    [Fact]
    public void Success_Should_CreateSuccessfulResult_With_NoError()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_Should_CreateFailedResult_With_GivenError()
    {
        var error = Error.Conflict("Code", "Description");

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Success_OfT_Should_CreateSuccessfulResult_With_Value()
    {
        var result = Result.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_OfT_Should_CreateFailedResult_With_GivenError()
    {
        var error = Error.NotFound("Code", "Description");

        var result = Result.Failure<int>(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Value_Should_Throw_When_ResultIsFailure()
    {
        var result = Result.Failure<int>(Error.NotFound("Code", "Description"));

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ImplicitOperator_Should_CreateSuccessfulResult_From_Value()
    {
        Result<int> result = 42;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }
}
