using FluentAssertions;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.UnitTests.Domain.Results;

public class ValidationResultTests
{
    [Fact]
    public void WithErrors_Should_CreateFailedResult_With_ValidationErrorType_And_GivenErrors()
    {
        var errors = new[]
        {
            Error.Validation("Name", "Name is required"),
            Error.Validation("Email", "Email is invalid")
        };

        var result = ValidationResult.WithErrors(errors);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void WithErrors_OfT_Should_CreateFailedResult_With_ValidationErrorType_And_GivenErrors()
    {
        var errors = new[] { Error.Validation("Name", "Name is required") };

        var result = ValidationResult<string>.WithErrors(errors);

        result.IsSuccess.Should().BeFalse();
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.Validation);
        result.Errors.Should().BeEquivalentTo(errors);
    }

    [Fact]
    public void Value_Should_Throw_When_ValidationResultOfT()
    {
        var result = ValidationResult<string>.WithErrors([Error.Validation("Name", "Name is required")]);

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }
}
