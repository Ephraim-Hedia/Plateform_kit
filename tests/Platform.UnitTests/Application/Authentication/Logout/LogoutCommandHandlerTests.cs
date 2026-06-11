using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Authentication.Logout;
using Platform.Domain.Errors;
using Platform.UnitTests.Application.Authentication.TestDoubles;

namespace Platform.UnitTests.Application.Authentication.Logout;

public class LogoutCommandHandlerTests
{
    private readonly TestApplicationDbContext _dbContext = CreateDbContext();
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _handler = new LogoutCommandHandler(_dbContext);
    }

    [Fact]
    public async Task Handle_Should_RevokeToken_WhenTokenIsActive()
    {
        var refreshToken = Platform.Domain.Entities.RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new LogoutCommand(refreshToken.Token), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var storedToken = await _dbContext.RefreshTokens.SingleAsync(rt => rt.Id == refreshToken.Id);
        storedToken.IsRevoked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTokenDoesNotExist()
    {
        var result = await _handler.Handle(new LogoutCommand("non-existent-token"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.InvalidRefreshToken);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenTokenAlreadyRevoked()
    {
        var refreshToken = Platform.Domain.Entities.RefreshToken.Create(Guid.NewGuid(), DateTimeOffset.UtcNow.AddDays(7));
        refreshToken.Revoke();
        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new LogoutCommand(refreshToken.Token), CancellationToken.None);

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
