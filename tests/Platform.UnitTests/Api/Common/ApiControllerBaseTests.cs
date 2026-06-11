using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Platform.API.Common;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.UnitTests.Api.Common;

public class ApiControllerBaseTests
{
    private sealed class TestController : ApiControllerBase
    {
        public IActionResult Fail(Result result) => HandleFailure(result);
    }

    [Theory]
    [MemberData(nameof(FailureResults))]
    public void HandleFailure_Should_MapErrorTypeToStatusCode(Result result, int expectedStatusCode)
    {
        var controller = CreateController();

        var actionResult = controller.Fail(result);

        actionResult.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(expectedStatusCode);
    }

    [Fact]
    public void HandleFailure_Should_ReturnValidationProblem_When_ResultIsValidationResult()
    {
        var controller = CreateController();
        var result = ValidationResult.WithErrors([Error.Validation("Name", "Name is required")]);

        var actionResult = controller.Fail(result);

        var objectResult = actionResult.Should().BeAssignableTo<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        objectResult.Value.Should().BeOfType<ValidationProblemDetails>()
            .Which.Errors.Should().ContainKey("Name");
    }

    [Fact]
    public void HandleFailure_Should_Throw_When_ResultIsSuccess()
    {
        var controller = CreateController();

        var act = () => controller.Fail(Result.Success());

        act.Should().Throw<InvalidOperationException>();
    }

    public static TheoryData<Result, int> FailureResults() => new()
    {
        { Result.Failure(Error.NotFound("Code", "Description")), StatusCodes.Status404NotFound },
        { Result.Failure(Error.Conflict("Code", "Description")), StatusCodes.Status409Conflict },
        { Result.Failure(Error.Unauthorized("Code", "Description")), StatusCodes.Status401Unauthorized },
        { Result.Failure(Error.Forbidden("Code", "Description")), StatusCodes.Status403Forbidden },
        { Result.Failure(Error.Failure("Code", "Description")), StatusCodes.Status500InternalServerError }
    };

    private static TestController CreateController()
    {
        var services = new ServiceCollection();
        services.AddMvc();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        return new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }
}
