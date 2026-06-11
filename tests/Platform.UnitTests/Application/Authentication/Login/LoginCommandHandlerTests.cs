using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using Platform.Application.Abstractions;
using Platform.Application.Authentication;
using Platform.Application.Authentication.Login;
using Platform.Domain.Errors;
using Platform.Domain.Results;
using Platform.UnitTests.Application.Authentication.TestDoubles;

namespace Platform.UnitTests.Application.Authentication.Login;

public class LoginCommandHandlerTests
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

    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(_identityService, _tokenGenerator, _dbContext, _jwtOptions);
    }

    [Fact]
    public async Task Handle_Should_ReturnTokens_WhenCredentialsAreValid()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "user@example.com", ["User"]);
        _identityService.ValidateCredentialsAsync(user.Email, "Password123!", Arg.Any<CancellationToken>())
            .Returns(Result.Success(user));
        _tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Roles).Returns("access-token");

        var result = await _handler.Handle(new LoginCommand(user.Email, "Password123!"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("access-token");
        result.Value.RefreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Handle_Should_PersistRefreshToken_WhenCredentialsAreValid()
    {
        var user = new AuthenticatedUser(Guid.NewGuid(), "user@example.com", ["User"]);
        _identityService.ValidateCredentialsAsync(user.Email, "Password123!", Arg.Any<CancellationToken>())
            .Returns(Result.Success(user));
        _tokenGenerator.GenerateAccessToken(user.Id, user.Email, user.Roles).Returns("access-token");

        var result = await _handler.Handle(new LoginCommand(user.Email, "Password123!"), CancellationToken.None);

        var storedToken = await _dbContext.RefreshTokens.SingleAsync();
        storedToken.UserId.Should().Be(user.Id);
        storedToken.Token.Should().Be(result.Value.RefreshToken);
        storedToken.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenCredentialsAreInvalid()
    {
        _identityService.ValidateCredentialsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthenticatedUser>(AuthenticationErrors.InvalidCredentials));

        var result = await _handler.Handle(new LoginCommand("user@example.com", "wrong-password"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_Should_NotPersistRefreshToken_WhenCredentialsAreInvalid()
    {
        _identityService.ValidateCredentialsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthenticatedUser>(AuthenticationErrors.InvalidCredentials));

        await _handler.Handle(new LoginCommand("user@example.com", "wrong-password"), CancellationToken.None);

        (await _dbContext.RefreshTokens.AnyAsync()).Should().BeFalse();
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
