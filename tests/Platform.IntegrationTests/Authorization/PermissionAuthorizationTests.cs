using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Authentication;
using Platform.Application.Permissions.GetPermissions;
using Platform.Application.Roles.GetRoles;
using Platform.Domain.Constants;
using Platform.Infrastructure.Identity;

namespace Platform.IntegrationTests.Authorization;

public class PermissionAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PermissionAuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPermissions_Should_ReturnUnauthorized_WithoutToken()
    {
        var response = await _client.GetAsync("/api/permissions");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPermissions_Should_ReturnForbidden_WhenUserHasNoPermissions()
    {
        var token = await RegisterAndLoginAsync($"{Guid.NewGuid()}@example.com");

        var response = await SendAsync(HttpMethod.Get, "/api/permissions", token);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetPermissions_Should_ReturnSeededPermissions_ForAdministrator()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        await RegisterAsync(email);
        await AddUserToRoleAsync(email, "Administrator");
        var token = await LoginAsync(email);

        var response = await SendAsync(HttpMethod.Get, "/api/permissions", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var permissions = await response.Content.ReadFromJsonAsync<List<PermissionDto>>(JsonOptions);
        permissions.Should().Contain(p => p.Code == Permissions.PermissionCatalog.View);
        permissions.Should().Contain(p => p.Code == Permissions.Roles.View);
        permissions.Should().Contain(p => p.Code == Permissions.Roles.Manage);
    }

    [Fact]
    public async Task GetRoles_Should_ReturnAdministratorRole_WithSeededPermissions()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        await RegisterAsync(email);
        await AddUserToRoleAsync(email, "Administrator");
        var token = await LoginAsync(email);

        var response = await SendAsync(HttpMethod.Get, "/api/roles", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>(JsonOptions);
        var adminRole = roles.Should().ContainSingle(r => r.Name == "Administrator").Subject;
        adminRole.Permissions.Should().Contain(Permissions.PermissionCatalog.View);
    }

    [Fact]
    public async Task AssignPermissions_Should_TakeEffectImmediately_WithoutTokenRefresh()
    {
        var adminEmail = $"{Guid.NewGuid()}@example.com";
        await RegisterAsync(adminEmail);
        await AddUserToRoleAsync(adminEmail, "Administrator");
        var adminToken = await LoginAsync(adminEmail);

        var testerRoleId = await CreateRoleAsync($"Tester-{Guid.NewGuid():N}");
        var testerEmail = $"{Guid.NewGuid()}@example.com";
        await RegisterAsync(testerEmail);
        await AddUserToRoleAsync(testerEmail, await GetRoleNameAsync(testerRoleId));
        var testerToken = await LoginAsync(testerEmail);

        var initialResponse = await SendAsync(HttpMethod.Get, "/api/permissions", testerToken);
        initialResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var assignResponse = await SendAsync(
            HttpMethod.Put,
            $"/api/roles/{testerRoleId}/permissions",
            adminToken,
            new { PermissionCodes = new[] { Permissions.PermissionCatalog.View } });
        assignResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var followUpResponse = await SendAsync(HttpMethod.Get, "/api/permissions", testerToken);
        followUpResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignPermissions_Should_ReturnNotFound_WhenRoleDoesNotExist()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        await RegisterAsync(email);
        await AddUserToRoleAsync(email, "Administrator");
        var token = await LoginAsync(email);

        var response = await SendAsync(
            HttpMethod.Put,
            $"/api/roles/{Guid.NewGuid()}/permissions",
            token,
            new { PermissionCodes = new[] { Permissions.PermissionCatalog.View } });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignPermissions_Should_ReturnNotFound_WhenPermissionCodeIsUnknown()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        await RegisterAsync(email);
        await AddUserToRoleAsync(email, "Administrator");
        var token = await LoginAsync(email);

        var roleId = await CreateRoleAsync($"Role-{Guid.NewGuid():N}");

        var response = await SendAsync(
            HttpMethod.Put,
            $"/api/roles/{roleId}/permissions",
            token,
            new { PermissionCodes = new[] { "Unknown.Permission" } });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AssignPermissions_Should_ReturnForbidden_WhenUserLacksManagePermission()
    {
        var token = await RegisterAndLoginAsync($"{Guid.NewGuid()}@example.com");

        var response = await SendAsync(
            HttpMethod.Put,
            $"/api/roles/{Guid.NewGuid()}/permissions",
            token,
            new { PermissionCodes = Array.Empty<string>() });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, string token, object? body = null)
    {
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        return await _client.SendAsync(request);
    }

    private async Task RegisterAsync(string email, string password = "Password123!")
    {
        await _client.PostAsJsonAsync("/api/auth/register", new { Email = email, Password = password });
    }

    private async Task<string> LoginAsync(string email, string password = "Password123!")
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { Email = email, Password = password });
        var tokens = await response.Content.ReadFromJsonAsync<AuthTokensResponse>(JsonOptions);

        return tokens!.AccessToken;
    }

    private async Task<string> RegisterAndLoginAsync(string email, string password = "Password123!")
    {
        await RegisterAsync(email, password);

        return await LoginAsync(email, password);
    }

    private async Task AddUserToRoleAsync(string email, string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.FindByEmailAsync(email);
        await userManager.AddToRoleAsync(user!, roleName);
    }

    private async Task<Guid> CreateRoleAsync(string roleName)
    {
        using var scope = _factory.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var role = new ApplicationRole { Id = Guid.NewGuid(), Name = roleName, NormalizedName = roleName.ToUpperInvariant() };
        await roleManager.CreateAsync(role);

        return role.Id;
    }

    private async Task<string> GetRoleNameAsync(Guid roleId)
    {
        using var scope = _factory.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        var role = await roleManager.FindByIdAsync(roleId.ToString());

        return role!.Name!;
    }
}
