using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Platform.API.Authentication;
using Platform.Application.Abstractions;
using Platform.Application.Authentication;
using Platform.Application.Authentication.Login;
using Platform.Application.Authentication.Logout;
using Platform.Application.Authentication.RefreshToken;
using Platform.Application.Authentication.Register;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.UnitTests.Api.Authentication;

public class AuthControllerTests
{
    private readonly ISender _sender = Substitute.For<ISender>();
    private readonly ICurrentUser _currentUser = Substitute.For<ICurrentUser>();

    [Fact]
    public async Task Register_Should_ReturnOk_WhenSucceeded()
    {
        var userId = Guid.NewGuid();
        _sender.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userId));

        var controller = CreateController();

        var actionResult = await controller.Register(new RegisterCommand("user@example.com", "Password123!"), CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { UserId = userId });
    }

    [Fact]
    public async Task Register_Should_ReturnProblem_WhenFailed()
    {
        _sender.Send(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(AuthenticationErrors.EmailAlreadyExists));

        var controller = CreateController();

        var actionResult = await controller.Register(new RegisterCommand("user@example.com", "Password123!"), CancellationToken.None);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status409Conflict);
    }

    [Fact]
    public async Task Login_Should_ReturnOk_WithTokens_WhenSucceeded()
    {
        var tokens = new AuthTokensResponse("access-token", "refresh-token", DateTimeOffset.UtcNow.AddDays(7));
        _sender.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(tokens));

        var controller = CreateController();

        var actionResult = await controller.Login(new LoginCommand("user@example.com", "Password123!"), CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(tokens);
    }

    [Fact]
    public async Task Login_Should_ReturnProblem_WhenCredentialsAreInvalid()
    {
        _sender.Send(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthTokensResponse>(AuthenticationErrors.InvalidCredentials));

        var controller = CreateController();

        var actionResult = await controller.Login(new LoginCommand("user@example.com", "wrong-password"), CancellationToken.None);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnOk_WithTokens_WhenSucceeded()
    {
        var tokens = new AuthTokensResponse("new-access-token", "new-refresh-token", DateTimeOffset.UtcNow.AddDays(7));
        _sender.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(tokens));

        var controller = CreateController();

        var actionResult = await controller.RefreshToken(new RefreshTokenCommand("old-refresh-token"), CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().Be(tokens);
    }

    [Fact]
    public async Task RefreshToken_Should_ReturnProblem_WhenTokenIsInvalid()
    {
        _sender.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthTokensResponse>(AuthenticationErrors.InvalidRefreshToken));

        var controller = CreateController();

        var actionResult = await controller.RefreshToken(new RefreshTokenCommand("invalid-token"), CancellationToken.None);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public async Task Logout_Should_ReturnNoContent_WhenSucceeded()
    {
        _sender.Send(Arg.Any<LogoutCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var controller = CreateController();

        var actionResult = await controller.Logout(new LogoutCommand("refresh-token"), CancellationToken.None);

        actionResult.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Logout_Should_ReturnProblem_WhenTokenIsInvalid()
    {
        _sender.Send(Arg.Any<LogoutCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(AuthenticationErrors.InvalidRefreshToken));

        var controller = CreateController();

        var actionResult = await controller.Logout(new LogoutCommand("invalid-token"), CancellationToken.None);

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
    }

    [Fact]
    public void Me_Should_ReturnOk_WithCurrentUserId()
    {
        var userId = Guid.NewGuid();
        _currentUser.UserId.Returns(userId);

        var controller = CreateController();

        var actionResult = controller.Me();

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(new { UserId = (Guid?)userId });
    }

    private AuthController CreateController()
    {
        var services = new ServiceCollection();
        services.AddMvc();

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider()
        };

        return new AuthController(_sender, _currentUser)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };
    }
}
