using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Platform.Domain.Errors;
using Platform.Domain.Results;

namespace Platform.API.Common;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleFailure(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("HandleFailure cannot be called for a successful result.");
        }

        if (result is IValidationResult validationResult)
        {
            return ValidationProblem(CreateModelState(validationResult.Errors));
        }

        return Problem(
            statusCode: MapErrorTypeToStatusCode(result.Error.Type),
            title: result.Error.Code,
            detail: result.Error.Description);
    }

    private static int MapErrorTypeToStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        _ => StatusCodes.Status500InternalServerError
    };

    private static ModelStateDictionary CreateModelState(Error[] errors)
    {
        var modelState = new ModelStateDictionary();

        foreach (var error in errors)
        {
            modelState.AddModelError(error.Code, error.Description);
        }

        return modelState;
    }
}
