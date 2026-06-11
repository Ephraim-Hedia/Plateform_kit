using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Platform.API.Common;
using Platform.API.RateLimiting;
using Platform.Application.Abstractions;
using Platform.Application.Authentication.Login;
using Platform.Application.Authentication.Logout;
using Platform.Application.Authentication.RefreshToken;
using Platform.Application.Authentication.Register;

namespace Platform.API.Authentication;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public sealed class AuthController(ISender sender, ICurrentUser currentUser) : ApiControllerBase
{
    [EnableRateLimiting(RateLimitingPolicies.Register)]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(new { UserId = result.Value }) : HandleFailure(result);
    }

    [EnableRateLimiting(RateLimitingPolicies.Login)]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [EnableRateLimiting(RateLimitingPolicies.RefreshToken)]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : HandleFailure(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : HandleFailure(result);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() => Ok(new { currentUser.UserId });
}
