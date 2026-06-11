using FluentValidation;

namespace Platform.UnitTests.Application.Common.Behaviors.TestDoubles;

public sealed class TestQueryValidator : AbstractValidator<TestQuery>
{
    public TestQueryValidator()
    {
        RuleFor(query => query.Name).NotEmpty();
    }
}
