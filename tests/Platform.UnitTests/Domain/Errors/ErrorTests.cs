using FluentAssertions;
using Platform.Domain.Errors;

namespace Platform.UnitTests.Domain.Errors;

public class ErrorTests
{
    [Theory]
    [InlineData(ErrorType.Failure)]
    [InlineData(ErrorType.Validation)]
    [InlineData(ErrorType.NotFound)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.Unauthorized)]
    [InlineData(ErrorType.Forbidden)]
    public void FactoryMethods_Should_CreateErrorWithExpectedType(ErrorType expectedType)
    {
        var error = expectedType switch
        {
            ErrorType.Failure => Error.Failure("Code", "Description"),
            ErrorType.Validation => Error.Validation("Code", "Description"),
            ErrorType.NotFound => Error.NotFound("Code", "Description"),
            ErrorType.Conflict => Error.Conflict("Code", "Description"),
            ErrorType.Unauthorized => Error.Unauthorized("Code", "Description"),
            ErrorType.Forbidden => Error.Forbidden("Code", "Description"),
            _ => throw new ArgumentOutOfRangeException(nameof(expectedType))
        };

        error.Code.Should().Be("Code");
        error.Description.Should().Be("Description");
        error.Type.Should().Be(expectedType);
    }

    [Fact]
    public void None_Should_HaveEmptyCodeAndDescription()
    {
        Error.None.Code.Should().BeEmpty();
        Error.None.Description.Should().BeEmpty();
        Error.None.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public void Errors_Should_BeEqual_When_CodeDescriptionAndTypeMatch()
    {
        var first = Error.NotFound("User.NotFound", "User was not found");
        var second = Error.NotFound("User.NotFound", "User was not found");

        first.Should().Be(second);
    }
}
