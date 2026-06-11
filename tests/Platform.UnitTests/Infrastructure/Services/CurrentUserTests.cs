using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Platform.Infrastructure.Services;

namespace Platform.UnitTests.Infrastructure.Services;

public class CurrentUserTests
{
    [Fact]
    public void UserId_Should_BeNull_And_NotAuthenticated_When_NoHttpContext()
    {
        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns((HttpContext?)null);

        var currentUser = new CurrentUser(accessor);

        currentUser.UserId.Should().BeNull();
        currentUser.IsAuthenticated.Should().BeFalse();
    }

    [Fact]
    public void UserId_Should_ReturnUserId_And_Authenticated_When_ClaimPresent()
    {
        var userId = Guid.NewGuid();
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
            authenticationType: "Test");

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        });

        var currentUser = new CurrentUser(accessor);

        currentUser.UserId.Should().Be(userId);
        currentUser.IsAuthenticated.Should().BeTrue();
    }

    [Fact]
    public void UserId_Should_BeNull_When_NoUserIdClaim()
    {
        var identity = new ClaimsIdentity();

        var accessor = Substitute.For<IHttpContextAccessor>();
        accessor.HttpContext.Returns(new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        });

        var currentUser = new CurrentUser(accessor);

        currentUser.UserId.Should().BeNull();
        currentUser.IsAuthenticated.Should().BeFalse();
    }
}
