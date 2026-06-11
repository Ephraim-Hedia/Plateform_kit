using FluentAssertions;
using NSubstitute;
using Platform.Application.Abstractions;
using Platform.Application.Authentication.Register;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.UnitTests.Application.Authentication.Register;

public class RegisterCommandHandlerTests
{
    private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_identityService);
    }

    [Fact]
    public async Task Handle_Should_ReturnUserId_WhenCreationSucceeds()
    {
        var userId = Guid.NewGuid();
        _identityService.CreateUserAsync("user@example.com", "Password123!", Arg.Any<CancellationToken>())
            .Returns(Result.Success(userId));

        var result = await _handler.Handle(new RegisterCommand("user@example.com", "Password123!"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(userId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenEmailAlreadyExists()
    {
        _identityService.CreateUserAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(AuthenticationErrors.EmailAlreadyExists));

        var result = await _handler.Handle(new RegisterCommand("user@example.com", "Password123!"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthenticationErrors.EmailAlreadyExists);
    }
}
