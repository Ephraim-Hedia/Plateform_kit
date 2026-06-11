using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Platform.Domain.Errors;
using Platform.Infrastructure.Identity;
using Platform.Infrastructure.Persistence;

namespace Platform.UnitTests.Infrastructure.Identity;

public class RoleServiceTests
{
    private readonly ApplicationDbContext _dbContext = CreateDbContext();
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        var roleStore = new RoleStore<ApplicationRole, ApplicationDbContext, Guid>(_dbContext);

        _roleManager = new RoleManager<ApplicationRole>(
            roleStore,
            [],
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            NullLogger<RoleManager<ApplicationRole>>.Instance);

        _roleService = new RoleService(_roleManager);
    }

    [Fact]
    public async Task GetRolesAsync_Should_ReturnEmptyList_WhenNoRolesExist()
    {
        var result = await _roleService.GetRolesAsync(CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRolesAsync_Should_ReturnAllCreatedRoles()
    {
        await _roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid(), Name = "Administrator", NormalizedName = "ADMINISTRATOR" });
        await _roleManager.CreateAsync(new ApplicationRole { Id = Guid.NewGuid(), Name = "Manager", NormalizedName = "MANAGER" });

        var result = await _roleService.GetRolesAsync(CancellationToken.None);

        result.Should().HaveCount(2);
        result.Should().Contain(role => role.Name == "Administrator");
        result.Should().Contain(role => role.Name == "Manager");
    }

    [Fact]
    public async Task GetRoleAsync_Should_ReturnRole_WhenRoleExists()
    {
        var role = new ApplicationRole { Id = Guid.NewGuid(), Name = "Manager", NormalizedName = "MANAGER" };
        await _roleManager.CreateAsync(role);

        var result = await _roleService.GetRoleAsync(role.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(role.Id);
        result.Value.Name.Should().Be("Manager");
    }

    [Fact]
    public async Task GetRoleAsync_Should_ReturnFailure_WhenRoleDoesNotExist()
    {
        var result = await _roleService.GetRoleAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AuthorizationErrors.RoleNotFound);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
