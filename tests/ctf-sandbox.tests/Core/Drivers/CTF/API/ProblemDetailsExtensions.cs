// using Microsoft.AspNetCore.Mvc;

// namespace ctf_sandbox.tests.Core.Drivers.CTF.API;

// /// <summary>Maps REST API problem details onto the API-agnostic <see cref="Result"/>/<see cref="ResultError"/> DTOs.</summary>
// public static class ProblemDetailsExtensions
// {
//     public static ResultError ToResultError(this ProblemDetails problemDetails) =>
//         new(problemDetails.Detail ?? problemDetails.Title);

//     public static ResultError ToResultError(this ValidationProblemDetails validationProblemDetails) =>
//         new(validationProblemDetails.Detail ?? validationProblemDetails.Title,
//             validationProblemDetails.Errors.ToDictionary(e => e.Key, e => e.Value));

//     public static Result ToFailureResult(this ProblemDetails problemDetails) =>
//         Result.Failure(problemDetails.ToResultError());

//     public static Result<TValue> ToFailureResult<TValue>(this ProblemDetails problemDetails) =>
//         Result<TValue>.Failure(problemDetails.ToResultError());

//     public static Result ToFailureResult(this ValidationProblemDetails validationProblemDetails) =>
//         Result.Failure(validationProblemDetails.ToResultError());

//     public static Result<TValue> ToFailureResult<TValue>(this ValidationProblemDetails validationProblemDetails) =>
//         Result<TValue>.Failure(validationProblemDetails.ToResultError());
// }
