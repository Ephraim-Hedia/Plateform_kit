using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Platform.Application.Permissions.GetPermissions;
using Platform.Domain.Entities;
using Platform.UnitTests.Application.Authentication.TestDoubles;

namespace Platform.UnitTests.Application.Permissions.GetPermissions;

public class GetPermissionsQueryHandlerTests
{
    private readonly TestApplicationDbContext _dbContext = CreateDbContext();
    private readonly GetPermissionsQueryHandler _handler;

    public GetPermissionsQueryHandlerTests()
    {
        _handler = new GetPermissionsQueryHandler(_dbContext);
    }

    [Fact]
    public async Task Handle_Should_ReturnPermissions_OrderedByCode()
    {
        _dbContext.Permissions.AddRange(
            Permission.Create("Roles.Manage", "Manage Roles"),
            Permission.Create("Permissions.View", "View Permissions"),
            Permission.Create("Roles.View", "View Roles"));
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        var result = await _handler.Handle(new GetPermissionsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(p => p.Code).Should().ContainInOrder("Permissions.View", "Roles.Manage", "Roles.View");
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoPermissionsExist()
    {
        var result = await _handler.Handle(new GetPermissionsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestApplicationDbContext(options);
    }
}
