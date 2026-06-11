using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Platform.Infrastructure.Authorization;

namespace Platform.UnitTests.Infrastructure.Authorization;

public class PermissionAuthorizationPolicyProviderTests
{
    private readonly PermissionAuthorizationPolicyProvider _provider = new(Options.Create(new AuthorizationOptions()));

    [Fact]
    public async Task GetPolicyAsync_Should_ReturnPermissionPolicy_ForPermissionPrefixedName()
    {
        var policy = await _provider.GetPolicyAsync("Permission:Roles.View");

        policy.Should().NotBeNull();

        var requirement = policy!.Requirements.OfType<PermissionRequirement>().Single();
        requirement.Permission.Should().Be("Roles.View");
    }

    [Fact]
    public async Task GetPolicyAsync_Should_FallBackToDefaultProvider_ForOtherPolicyNames()
    {
        var options = new AuthorizationOptions();
        options.AddPolicy("CustomPolicy", policy => policy.RequireAuthenticatedUser());
        var provider = new PermissionAuthorizationPolicyProvider(Options.Create(options));

        var policy = await provider.GetPolicyAsync("CustomPolicy");

        policy.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPolicyAsync_Should_ReturnNull_ForUnknownPolicyName()
    {
        var policy = await _provider.GetPolicyAsync("Unknown");

        policy.Should().BeNull();
    }
}
