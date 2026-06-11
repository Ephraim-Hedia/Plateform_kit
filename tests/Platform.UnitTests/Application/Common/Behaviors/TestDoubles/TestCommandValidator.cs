using FluentValidation;

namespace Platform.UnitTests.Application.Common.Behaviors.TestDoubles;

public sealed class TestCommandValidator : AbstractValidator<TestCommand>
{
    public TestCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty();
    }
}
