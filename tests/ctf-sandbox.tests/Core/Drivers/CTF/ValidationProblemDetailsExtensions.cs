using Microsoft.AspNetCore.Mvc;

namespace ctf_sandbox.tests.Core.Drivers.CTF;

public static class ValidationProblemDetailsExtensions
{

    public static SystemError MapError(this ValidationProblemDetails validationProblemDetails)
    {
        var message = validationProblemDetails.Detail ?? "Request failed";
        if (validationProblemDetails.Errors != null && validationProblemDetails.Errors.Count > 0)
        {
            var fieldErrors = validationProblemDetails.Errors
                .SelectMany(e => (e.Value ?? [string.Empty])
                    .Select(message => new SystemError.FieldError(e.Key ?? "unknown", message)))
                .ToList();
            return SystemError.Of(message, fieldErrors.AsReadOnly());
        }
        return SystemError.Of(message);
    }    
    
    // public static ResultError ToResultError(this ProblemDetails problemDetails) =>
    //     new(problemDetails.Detail ?? problemDetails.Title);

    // public static ResultError ToResultError(this ValidationProblemDetails validationProblemDetails) =>
    //     new(validationProblemDetails.Detail ?? validationProblemDetails.Title,
    //         validationProblemDetails.Errors.ToDictionary(e => e.Key, e => e.Value));

    // public static Result ToFailureResult(this ProblemDetails problemDetails) =>
    //     Result.Failure(problemDetails.ToResultError());

    // public static Result<TValue> ToFailureResult<TValue>(this ProblemDetails problemDetails) =>
    //     Result<TValue>.Failure(problemDetails.ToResultError());

    // public static Result ToFailureResult(this ValidationProblemDetails validationProblemDetails) =>
    //     Result.Failure(validationProblemDetails.ToResultError());

    // public static Result<TValue> ToFailureResult<TValue>(this ValidationProblemDetails validationProblemDetails) =>
    //     Result<TValue>.Failure(validationProblemDetails.ToResultError());
}
