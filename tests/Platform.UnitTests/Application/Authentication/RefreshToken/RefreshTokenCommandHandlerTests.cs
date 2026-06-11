using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using Platform.Application.Abstractions;
using Platform.Application.Authentication;
using Platform.Application.Authentication.RefreshToken;
using Platform.Domain.Errors;
using Platform.Domain.Results;
using Platform.UnitTests.Application.Authentication.TestDoubles;

namespace Platform.UnitTests.Application.Authentication.RefreshToken;

public class RefreshTokenCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly IJwtTokenGenerator _tokenGenerator = Substitute.For<IJwtTokenGenerator>();
    private readonly TestApplicationDbContext _dbContext = CreateDbContext();
    private readonly IOptions<JwtOptions> _jwtOptions = Options.Create(new JwtOptions
    {
        Secret = "unit-test-secret",
        Issuer = "unit-test-issuer",
        Audience = "unit-test-audience",
        AccessTokenExpirationMinutes = 15,
        RefreshTokenExpirationDays = 7
    });

    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _handler = new RefreshTokenCommandHandler(_dbContext, _identityService, _tokenGenerator, _jwtOptions);
    }

    [Fact]
    public async Task Handle_Should_RotateToken_WhenExistingTokenIsActive()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "user@example.com", ["User"]);
        var existingToken = Platform.Domain.Entities.RefreshToken.Create(user.Id, DateTimeOffset.UtcNow.AddDays(7));
        _dbContext.RefreshTokens.Add(existingToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        _identityService.GetUserAsync(user.Id, Arg.Any<CancellationToken>()).Returns(Result.Success(user));
        _tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Roles).Returns("new-access-token");

        var result = await _handler.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("new-access-token");
        result.Value.RefreshToken.Should().NotBe(existingToken.Token);
    }

    [Fact]
    public async Task Handle_Should_RevokeOldToken_AndLinkToNewToken()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "user@example.com", ["User"]);
        var existingToken = Platform.Domain.Entities.RefreshToken.Create(user.Id, DateTimeOffset.UtcNow.AddDays(7));
        _dbContext.RefreshTokens.Add(existingToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        _identityService.GetUserAsync(user.Id, Arg.Any<CancellationToken>()).Returns(Result.Success(user));
        _tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Roles).Returns("new-access-token");

        var result = await _handler.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        var oldToken = await _dbContext.RefreshTokens.SingleAsync(rt => rt.Id == existingToken.Id);
        oldToken.IsRevoked.Should().BeTrue();
        oldToken.ReplacedByToken.Should().Be(result.Value.RefreshToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTokenDoesNotExist()
    {
        var result = await _handler.Handle(new RefreshTokenCommand("non-existent-token"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTokenIsRevoked()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "user@example.com", ["User"]);
        var existingToken = Platform.Domain.Entities.RefreshToken.Create(user.Id, DateTimeOffset.UtcNow.AddDays(7));
        existingToken.Revoke();
        _dbContext.RefreshTokens.Add(existingToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTokenIsExpired()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "user@example.com", ["User"]);
        var existingToken = Platform.Domain.Entities.RefreshToken.Create(user.Id, DateTimeOffset.UtcNow.AddDays(-1));
        _dbContext.RefreshTokens.Add(existingToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new RefreshTokenCommand(existingToken.Token), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidRefreshToken);
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
