using FluentValidation;
using MediatR;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var errors = validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .Select(failure => Error.Validation(failure.PropertyName, failure.ErrorMessage))
            .Distinct()
            .ToArray();

        if (errors.Length == 0)
        {
            return await next();
        }

        return CreateValidationResult<TResponse>(errors);
    }

    private static TResult CreateValidationResult<TResult>(Error[] errors)
        where TResult : Result
    {
        if (typeof(TResult) == typeof(Result))
        {
            return (TResult)(object)ValidationResult.WithErrors(errors);
        }

        var valueType = typeof(TResult).GetGenericArguments()[0];

        var validationResult = typeof(ValidationResult<>)
            .MakeGenericType(valueType)
            .GetMethod(nameof(ValidationResult<object>.WithErrors))!
            .Invoke(null, [errors]);

        return (TResult)validationResult!;
    }
}
