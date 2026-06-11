using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;

namespace Platform.ArchitectureTests;

public class DependencyRuleTests
{
    private static readonly Assembly DomainAssembly = Assembly.Load("Platform.Domain");
    private static readonly Assembly ApplicationAssembly = Assembly.Load("Platform.Application");
    private static readonly Assembly InfrastructureAssembly = Assembly.Load("Platform.Infrastructure");
    private static readonly Assembly ApiAssembly = Assembly.Load("Platform.API");

    [Fact]
    public void Domain_Should_Not_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Domain_Should_Not_DependOn_Api()
    {
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.API")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_Should_Not_DependOn_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.API")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_Should_Not_DependOn_Api()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOn("Platform.API")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
