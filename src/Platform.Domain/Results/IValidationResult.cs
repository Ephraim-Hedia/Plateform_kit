using Platform.Domain.Errors;

namespace Platform.Domain.Results;

public interface IValidationResult
{
    Error[] Errors { get; }
}
