using Platform.Domain.Errors;

namespace Platform.Domain.Results;

public sealed class ValidationResult : Result, IValidationResult
{
    private ValidationResult(Error[] errors)
        : base(false, Error.Validation("Validation.General", "One or more validation errors occurred."))
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationResult WithErrors(Error[] errors) => new(errors);
}
